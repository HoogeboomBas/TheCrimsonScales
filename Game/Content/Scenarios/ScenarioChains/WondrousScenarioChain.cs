using Godot;

public class WondrousScenarioChain : ScenarioChain
{
	public override ScenarioChain BaseScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string Name => "Wondrous Scenario Chain";
	public override Color Color => Color.FromHtml("93c07f");
}