public class PartyAchievementRequirement : ScenarioRequirement
{
	private readonly PartyAchievement _achievement;
	private readonly bool _complete;

	public PartyAchievementRequirement(PartyAchievement achievement, bool complete)
	{
		_achievement = achievement;
		_complete = complete;
	}

	public override bool GetMet(SavedCampaign savedCampaign)
	{
		return savedCampaign.HasPartyAchievement(_achievement) == _complete;
	}

	public override string NotMetMessage()
	{
		return $"You require the {_achievement.ToPrettyString()} achievement {(_complete ? "COMPLETE" : "INCOMPLETE")}.";
	}
}