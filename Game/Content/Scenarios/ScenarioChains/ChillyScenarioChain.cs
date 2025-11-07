using Godot;

public class ChillyScenarioChain : ScenarioChain
{
	public override ScenarioChain BaseScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override Color Color => Color.FromHtml("7fbbd2");
}