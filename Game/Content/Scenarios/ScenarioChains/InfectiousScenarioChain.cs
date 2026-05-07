using Godot;

public class InfectiousScenarioChain : ScenarioChain
{
	public override ScenarioChain BaseScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string Name => "Infectious Scenario Chain";
	public override Color Color => Color.FromHtml("93c07f");
}