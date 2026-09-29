public class ImpossibleRequirement : ScenarioRequirement
{
	public ImpossibleRequirement()
	{
	}

	public override bool GetMet(SavedCampaign savedCampaign)
	{
		return false;
	}

	public override string NotMetMessage(SavedCampaign savedCampaign)
	{
		return $"This scenario has not been implemented yet. Sorry for the inconvenience!";
	}
}