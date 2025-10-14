using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario017 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario017.tscn";
	public override int ScenarioNumber => 17;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario002>(true)];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAlLEnemiesScenarioGoals();

	public override string BGSPath => null;

	private bool _lootedTreasure;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(-1, OnTreasureLooted, null);
	}

	private async GDTask OnTreasureLooted(Character lootingCharacter)
	{
		_lootedTreasure = true;

		await GDTask.CompletedTask;
	}
}