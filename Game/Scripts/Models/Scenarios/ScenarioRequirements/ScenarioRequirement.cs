public abstract class ScenarioRequirement
{
	public abstract bool GetMet(SavedCampaign savedCampaign);
	public abstract string NotMetMessage();
}