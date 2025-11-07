using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario005 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario005.tscn";
	public override int ScenarioNumber => 5;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario006>()];

	protected override ScenarioGoals CreateScenarioGoals() => new KillSpecificEnemiesTypeGoals([ModelDB.Monster<GelatinousGiant>(), ModelDB.Monster<GelatinousGiantSecondStage>()], "Kill the Gelatinous Giant to win this scenario.");

	int _markersLeftToRemove;
	List<Marker> _markers = null;
	Dictionary<Marker, List<Hex>> _infectedWaterSources = [];
	Figure _gelatinousGiant = null;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText(
			$"All characters start with {Icons.Inline(Icons.GetCondition(Conditions.Infect))} as a scenario effect");

		//TODO: Scenario effect
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			await AbilityCmd.AddCondition(null, character, Conditions.Infect);
		}

		GameController.Instance.EndEvent += (backToTown, won, savedScenarioProgress) => 
		{ 
			if(won) { GameController.Instance.SavedCampaign.AddPartyAchievement(PartyAchievement.OozeDestroyed); }
		};
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		_gelatinousGiant = GameController.Instance.Map.Figures.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is GelatinousGiant).First();

		_markers = GameController.Instance.Map.Markers;

		foreach(Marker marker in _markers)
		{
			List<Hex> waterHexes = [marker.Hex];
			List<Hex> currentHexes = [marker.Hex];

			while(currentHexes.Count > 0)
			{
				Hex currentHex = currentHexes.First();
				foreach(Hex newHex in RangeHelper.GetHexesInRange(currentHex, 1, false, false).Except(waterHexes))
				{
					if(newHex.HasHexObjectOfType<Water>())
					{
						currentHexes.Add(newHex);
						waterHexes.Add(newHex);
					}
				}

				currentHexes.Remove(currentHex);
			}

			_infectedWaterSources.Add(marker, waterHexes);
		}

		_markersLeftToRemove = 4 - GameController.Instance.SavedCampaign.Characters.Count;

		UpdateScenarioText();

		int doorOpenedRoundNumber = GameController.Instance.ScenarioPhaseManager.RoundIndex + 1;
		int doorOpenedRoundNumberOddness = doorOpenedRoundNumber % 2;
		
		// Every other round spawn an ooze on the marker closest to the boss
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber % 2 != doorOpenedRoundNumberOddness,
			async parameters =>
			{
				await SpawnEliteBloodOoze();
			}
		);

		// When elite ooze is killed, prompt to destroy one of the markers and all connected water tiles
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Monster monsterFigure && 
				monsterFigure.MonsterModel is BloodOoze &&
				monsterFigure.MonsterType == MonsterType.Elite,
			async parameters =>
			{
				await DrainInfectedWater();
			}
		);
	}

	private async GDTask DrainInfectedWater()
	{
		Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(), hexes =>
		{
			hexes.AddRange(_markers.Select(marker => marker.Hex));
		}, mandatory: true, hintText: "Choose infected water to drain");

		// Hide the marker and remove it from the list
		Marker chosenMarker = _markers.First(marker => marker.Hex == chosenHex);
		_markers.Remove(chosenMarker);
		_markersLeftToRemove--;

		UpdateScenarioText();

		// Remove all connected water tiles
		await DrainAllConnectedWater(chosenMarker);

		// Drained C water markers, summon boss version that can be damaged and draws different abilities
		if(_markersLeftToRemove == 0)
		{
			await SummonSecondStageBoss();

			ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
			ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
		}
	}

	private async GDTask DrainAllConnectedWater(Marker infectedWaterMarker)
	{
		_infectedWaterSources[infectedWaterMarker].ForEach(async hex =>
		{
			await hex.GetHexObjectOfType<Water>().Destroy(forceDestroy: true);
		});

		_infectedWaterSources.Remove(infectedWaterMarker);
	}

	private async GDTask SpawnEliteBloodOoze()
	{
		// Sort the markers by distance to the boss
		_markers.Sort(Comparer<Marker>.Create(
			(marker0, marker1) => 
				RangeHelper.Distance(marker0.Hex, _gelatinousGiant.Hex) - RangeHelper.Distance(marker1.Hex, _gelatinousGiant.Hex)
		));

		// First see if there are unoccupied water hexes in water group with the closest marker
		// Then go to the next closest one
		Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(),
			list =>
			{
				foreach(Marker marker in _markers)
				{	
					if(marker.Hex.IsUnoccupied())
					{
						list.Add(marker.Hex);
						break;
					}
					
					if(_infectedWaterSources[marker].Any(marker => marker.IsUnoccupied()))
					{
						list.AddRange(_infectedWaterSources[marker]);
						break;
					}
				}
			}, true, $"Select where to summon the Elite Bloode Ooze"
		);

		if(chosenHex != null)
		{
			await AbilityCmd.SpawnMonster(ModelDB.Monster<BloodOoze>(), MonsterType.Elite, chosenHex);
		}
	}

	private async GDTask SummonSecondStageBoss()
	{
		int bossHealth = _gelatinousGiant.Health;
		Hex bossHex = _gelatinousGiant.Hex;

		ScenarioCheckEvents.SpawnCoinCheckEvent.Subscribe(this, 
			parameters => parameters.Figure == _gelatinousGiant,
			parameters => parameters.SetSpawnCoin(false));

		await _gelatinousGiant.Destroy(immediately: true);

		ScenarioCheckEvents.SpawnCoinCheckEvent.Unsubscribe(this);

		_gelatinousGiant = await AbilityCmd.SpawnMonster(ModelDB.Monster<GelatinousGiantSecondStage>(), MonsterType.Boss, bossHex);
		
		_gelatinousGiant.SetMaxHealth(bossHealth);
		_gelatinousGiant.SetHealth(bossHealth);
	}

	private void UpdateScenarioText()
	{
		if(_markersLeftToRemove > 0)
		{
			UpdateScenarioText(
				"Gelatinous Giant is immune to all negative conditions and cannot suffer damage from any source until the infected water has been drained." +
				System.Environment.NewLine + System.Environment.NewLine +
				"At the end of every other round, an Elite Blood Ooze spawns at an infected water source closest to the Gelationous Giant." +
				System.Environment.NewLine + System.Environment.NewLine +
				$"Drain {_markersLeftToRemove} more sources of infected water by killing Elite Blood Ooze.");
		}
		else
		{
			UpdateScenarioText("");
		}
	}
}
