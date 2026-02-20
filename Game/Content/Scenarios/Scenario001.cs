using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario001 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario001.tscn";
	public override int ScenarioNumber => 1;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario002>(true)];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();

	public override string BGSPath => "res://Audio/BGS/Forest Day.ogg";

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<DizzyingTincture>());
	}
}