using Fractural.Tasks;

public class TestScenario : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/TestScenario.tscn";
	public override int ScenarioNumber => 1;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	protected override ScenarioGoals CreateScenarioGoals() => new KillAlLEnemiesScenarioGoals();

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(0, ModelDB.Item<DizzyingTincture>());
	}
}