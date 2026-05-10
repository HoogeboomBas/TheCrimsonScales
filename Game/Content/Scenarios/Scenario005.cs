using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario005 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario005.tscn";

	public override int ScenarioNumber => 5;
	public override string Name => "Blood of the Oozes";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario006>()];

	public override string IntroductionText =>
		"""
		Following the dramatic infection of the guards, you proceed out of the gates with caution, extremely wary of the plagueridden monster, but it doesn’t take long before you find what you are looking for.

		Around the corner from the stricken guards, you see an alley which you know to normally be quiet. Tonight, it is far from quiet. You find it partially flooded, and more unusually, a collection of imps and demons block the way. Whether or not this leads to the plague carrier, this needs to be dealt with.
		""";

	public override string ConclusionText =>
		"""
		You dodge a last salvo from the Gelatinous Giant, before a final strike kills it. The gruesome rubbery structure is somehow still the same, but you can tell that it is now devoid of the evil life force that inhabited it. The sticky remains slide around and down the fountain. If it wasn’t in the water supply before, it is now.

		You race to the lake above Gloomhaven to try and purify the water at its source.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<GelatinousGiant>(),
		ModelDB.Monster<GelatinousGiantSecondStage>(),
		ModelDB.Monster<BloodOoze>(),
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<FlamingDrake>(),
		ModelDB.Monster<ToxicImp>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainPartyAchievementReward(PartyAchievement.OozeDestroyed)
	];

	private int _markersLeftToRemove;
	private List<Marker> _markers = null;
	private readonly Dictionary<Marker, List<Hex>> _infectedWaterSources = [];
	private Figure _gelatinousGiant = null;

	private CustomScenarioGoal _infectedWaterGoal;
	private ScenarioRule _gelatinousGiantInvulnerableRule;
	private ScenarioRule _spawnEliteBloodOozeRule;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Infect);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<GelatinousGiantSecondStage>(), specificCount: 1));

		GameController.Instance.EndEvent += (scenarioResult, savedScenarioProgress) =>
		{
			if(scenarioResult == ScenarioResult.Win)
			{
				GameController.Instance.SavedCampaign.AddPartyAchievement(PartyAchievement.OozeDestroyed);
			}
		};
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		_gelatinousGiant =
			GameController.Instance.Map.Figures.First(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is GelatinousGiant);

		_markers = GameController.Instance.Map.Markers.ToList();

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

		int characterCount = GameController.Instance.SavedCampaign.Characters.Count;
		_markersLeftToRemove = characterCount;

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
			parameters =>
				parameters.Figure is Monster monsterFigure &&
				monsterFigure.MonsterModel is BloodOoze &&
				monsterFigure.MonsterType == MonsterType.Elite,
			async parameters =>
			{
				await DrainInfectedWater();
			}
		);

		_infectedWaterGoal =
			await AddGoal(new CustomScenarioGoal(textParameters => $"Drain {characterCount} infected water tiles to make the Gelatinous Giant vulnerable.",
				hasProgress: true, maxProgress: characterCount, order: -1));

		_gelatinousGiantInvulnerableRule =
			AddScenarioRule(
				"The Gelatinous Giant is immune to all negative conditions and cannot suffer damage from any source until the infected water has been drained.");

		_spawnEliteBloodOozeRule =
			AddScenarioRule(
				"At the end of every other round, an Elite Blood Ooze spawns at an infected water source closest to the Gelatinous Giant.");

		await ShowText(
			"""
			You kick down the door to find more demons, and the source of the Bloody Pox—a huge Blood Ooze, its viscous shape surrounding a central fountain. You are momentarily distracted by the strange, pulsing shape—until a screech from one of the two Drakes snaps you back to reality...
			""");
	}

	private async GDTask DrainInfectedWater()
	{
		Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(), hexes =>
		{
			hexes.AddRange(_markers.Select(marker => marker.Hex));
		}, mandatory: true, hintText: "Choose infected water to drain");

		if(chosenHex == null)
		{
			return;
		}

		await _infectedWaterGoal.AdjustProgress(1);

		// Hide the marker and remove it from the list
		Marker chosenMarker = _markers.First(marker => marker.Hex == chosenHex);
		_markers.Remove(chosenMarker);
		_markersLeftToRemove--;

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
		foreach(Hex hex in _infectedWaterSources[infectedWaterMarker])
		{
			await hex.GetHexObjectOfType<Water>().Destroy(forceDestroy: true);
		}

		_infectedWaterSources.Remove(infectedWaterMarker);

		await GDTask.CompletedTask;
	}

	private async GDTask SpawnEliteBloodOoze()
	{
		// Sort the markers by distance to the boss
		_markers.Sort(Comparer<Marker>.Create((marker0, marker1) =>
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

					if(_infectedWaterSources[marker].Any(hex => hex.IsUnoccupied()))
					{
						list.AddRange(_infectedWaterSources[marker].Where(hex => hex.IsUnoccupied()));
						break;
					}
				}
			}, true, $"Select where to summon the Elite Blood Ooze"
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
			parameters => parameters.Dropper == _gelatinousGiant,
			parameters => parameters.SetCoinsToSpawn(0));

		await _gelatinousGiant.Destroy(immediately: true);

		ScenarioCheckEvents.SpawnCoinCheckEvent.Unsubscribe(this);

		_gelatinousGiant = await AbilityCmd.SummonMonster(ModelDB.Monster<GelatinousGiantSecondStage>(), MonsterType.Boss, bossHex);

		_gelatinousGiant.SetMaxHealth(bossHealth);
		_gelatinousGiant.SetHealth(bossHealth);

		_gelatinousGiantInvulnerableRule.Remove();
		_spawnEliteBloodOozeRule.Remove();
		AddScenarioRule("The Gelatinous Giant now draws from the Blood Ooze ability deck.");
	}
}