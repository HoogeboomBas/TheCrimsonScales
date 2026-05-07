using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainXPReward : SavedReward
{
	[JsonProperty]
	private int _xp;

	public override RewardType Type => RewardType.Immediate;

	public GainXPReward()
	{
	}

	public GainXPReward(int xp)
	{
		_xp = xp;
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain {Icons.Inline(Icons.XP, textParameters)}{_xp} each.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		foreach(SavedCharacter savedCharacter in savedCampaign.Characters)
		{
			savedCharacter.AddXP(_xp);
		}
	}
}