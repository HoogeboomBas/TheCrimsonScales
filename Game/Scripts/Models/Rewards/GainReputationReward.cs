using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainReputationReward : SavedReward
{
	[JsonProperty]
	private int _reputationAmount;

	public override RewardType Type => RewardType.Immediate;

	public GainReputationReward()
	{
	}

	public GainReputationReward(int reputationAmount)
	{
		_reputationAmount = reputationAmount;
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain {_reputationAmount} reputation.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		savedCampaign.AdjustReputation(_reputationAmount);
	}
}