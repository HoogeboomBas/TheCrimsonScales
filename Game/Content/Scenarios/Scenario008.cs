using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario008 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario008.tscn";
	public override int ScenarioNumber => 8;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario009>(true)];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<DrakesBlood>());
	}
}