public class Scenario006 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario006.tscn";
	public override int ScenarioNumber => 6;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new KillAlLEnemiesScenarioGoals();
}
