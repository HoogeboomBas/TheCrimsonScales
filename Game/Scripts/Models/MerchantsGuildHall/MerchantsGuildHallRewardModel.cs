using System.Collections.Generic;

public abstract class MerchantsGuildHallRewardModel : AbstractModel
{
	public abstract string GetDescription(RichTextParameters richTextParameters);

	public abstract List<SavedReward> GetRewards();
}