using System.Collections.Generic;

public abstract class EventChoiceModel : AbstractModel
{
	public abstract string ChoiceText { get; }

	public virtual void InitState(SavedEventState state, SavedCampaign savedCampaign)
	{
	}

	public virtual EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.Lost;

	public abstract string GetStoryText(SavedEventState state);

	public abstract List<SavedReward> GetRewards(SavedEventState state);
}