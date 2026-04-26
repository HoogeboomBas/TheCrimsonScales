using Godot;

public class ChillyScenarioChain : ScenarioChain
{
	public override ScenarioChain BaseScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string Name => "Chilly Scenario Chain";
	public override Color Color => Color.FromHtml("7fbbd2");
}