using Godot;

public class SailScenarioChain : ScenarioChain
{
	public override ScenarioChain BaseScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string Name => "Sail Scenario Chain";
	public override Color Color => Color.FromHtml("c4cec8");
}