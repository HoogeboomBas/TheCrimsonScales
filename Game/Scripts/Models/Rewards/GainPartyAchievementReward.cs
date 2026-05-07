using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainPartyAchievementReward : SavedReward
{
	[JsonProperty]
	private PartyAchievement _achievement;

	public override RewardType Type => RewardType.Immediate;

	public GainPartyAchievementReward()
	{
	}

	public GainPartyAchievementReward(PartyAchievement achievement)
	{
		_achievement = achievement;
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Party Achievement: “{_achievement.ToPrettyString()}”";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		savedCampaign.AddPartyAchievement(_achievement);
	}
}