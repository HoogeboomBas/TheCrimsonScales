using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Fractural.Tasks;

public class Scenario005 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario005.tscn";
	public override int ScenarioNumber => 5;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario006>()];

	protected override ScenarioGoals CreateScenarioGoals() => new KillSpecificEnemiesTypeGoals([ModelDB.Monster<GelatiousGiant>(), ModelDB.Monster<GelatinousGiantSecondStage>()], "Kill the Gelatinous Giant to win this scenario.");

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

		UpdateScenarioText(
			"Gelatinous Giant is immune to all negative conditions and cannot suffer damage from any source until the infected water has been drained." + 
			System.Environment.NewLine + System.Environment.NewLine +
			$"Whenever an elite Blood Ooze is killed, drain an infected water source {Icons.Inline(Icons.Marker(Marker.Type.a))}");

		Figure gelatinousGiant = GameController.Instance.Map.Figures.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is GelatinousGiant).First();

		List<Marker> markers = GameController.Instance.Map.Markers;

		int doorOpenedRoundNumber = GameController.Instance.ScenarioPhaseManager.RoundIndex + 1;
		int doorOpenedRoundNumberOddness = doorOpenedRoundNumber % 2;
		
		// Every other round spawn an ooze on the marker closest to the boss
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber % 2 != doorOpenedRoundNumberOddness,
			async parameters =>
			{
				markers.Sort(Comparer<Marker>.Create(
					(marker0, marker1) => 
						RangeHelper.Distance(marker0.Hex, gelatinousGiant.Hex) - RangeHelper.Distance(marker1.Hex, gelatinousGiant.Hex)
				));

				await AbilityCmd.SpawnMonster(ModelDB.Monster<BloodOoze>(), MonsterType.Elite, markers.First().Hex);
			}
		);

		// When elite ooze is killed, prompt to destroy one of the markers and all connected water tiles
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Monster monsterFigure && 
				monsterFigure.MonsterModel is BloodOoze &&
				monsterFigure.MonsterType == MonsterType.Elite,
			async parameters =>
			{
				Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(), hexes =>
				{
					hexes.AddRange(markers.Select(marker => marker.Hex));
				}, mandatory: true, hintText: "Choose infected water to drain");

				// Hide the marker and remove it from the list
				Marker chosenMarker = chosenHex.GetHexObjectOfType<Marker>();
				markers.Remove(chosenMarker);
				chosenMarker.Hide();

				// Remove all connected water tiles
				await DrainAllConnectedWater(chosenHex);

				// Drained C water markers, summon boss version that can be damaged and draws different abilities
				if(markers.Count == 4 - GameController.Instance.SavedCampaign.Characters.Count)
				{
					await SummonSecondStageBoss(gelatinousGiant);
				}
			}
		);
	}

	private async GDTask DrainAllConnectedWater(Hex waterHex)
	{
		List<Hex> waterHexes = [waterHex];
		while(waterHexes.Count > 0)
		{
			Hex currentHex = waterHexes.First();
			await currentHex.GetHexObjectOfType<Water>().Destroy(forceDestroy: true);
			foreach(Hex hex in RangeHelper.GetHexesInRange(currentHex, 1, false, false))
			{
				if(hex.HasHexObjectOfType<Water>())
				{
					waterHexes.Add(hex);
				}
			}
		}
	}

	private async GDTask SummonSecondStageBoss(Figure boss)
	{
		ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
		ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

		int bossHealth = boss.Health;
		Hex bossHex = boss.Hex;

		await boss.Destroy(immediately: true);

		Monster secondStageboss = await AbilityCmd.SpawnMonster(ModelDB.Monster<GelatinousGiantSecondStage>(), MonsterType.Boss, bossHex);
		secondStageboss.SetMaxHealth(bossHealth);
		secondStageboss.SetHealth(bossHealth);
	}
}

