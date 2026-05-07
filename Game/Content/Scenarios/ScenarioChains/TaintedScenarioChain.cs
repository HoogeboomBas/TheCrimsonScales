using Godot;

public class TaintedScenarioChain : ScenarioChain
{
	public override ScenarioChain BaseScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string Name => "Chilly Scenario Chain";
	public override Color Color => Color.FromHtml("b08674");
}