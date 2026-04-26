using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class LoseCheckmarkReward : SavedReward
{
	public override RewardType Type => RewardType.Immediate;

	public LoseCheckmarkReward()
	{
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Lose 1 {Icons.Inline(Icons.Checkmark, textParameters)} each.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		foreach(SavedCharacter character in savedCampaign.Characters)
		{
			character.RemoveCheckmark();
		}
	}
}