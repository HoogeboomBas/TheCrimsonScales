using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainCheckmarkReward : SavedReward
{
	public override RewardType Type => RewardType.Immediate;

	public GainCheckmarkReward()
	{
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain 1 {Icons.Inline(Icons.Checkmark, textParameters)} each.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		foreach(SavedCharacter character in savedCampaign.Characters)
		{
			character.AddCheckmark();
		}
	}
}