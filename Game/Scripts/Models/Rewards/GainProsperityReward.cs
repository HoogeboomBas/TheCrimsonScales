using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainProsperityReward : SavedReward
{
	[JsonProperty]
	private int _prosperity;

	public override RewardType Type => RewardType.Immediate;

	public GainProsperityReward()
	{
	}

	public GainProsperityReward(int prosperity)
	{
		_prosperity = prosperity;
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain {_prosperity} prosperity.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		savedCampaign.AdjustProsperity(_prosperity);
	}
}