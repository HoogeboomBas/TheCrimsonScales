using System.Collections.Generic;

public class GainProsperityMerchantsGuildHallReward : MerchantsGuildHallRewardModel
{
	public override string GetDescription(RichTextParameters richTextParameters) => "Gain +3 Prosperity";

	public override List<SavedReward> GetRewards() =>
	[
		new GainProsperityReward(3)
	];
}