using Godot;

public class SailScenarioChain : ScenarioChain
{
	public override ScenarioChain BaseScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override Color Color => Color.FromHtml("c4cec8");
}