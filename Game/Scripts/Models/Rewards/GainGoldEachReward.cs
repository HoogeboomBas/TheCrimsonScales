using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainGoldEachReward : SavedReward
{
	[JsonProperty]
	private int _goldAmount;

	public override RewardType Type => RewardType.Immediate;

	public GainGoldEachReward()
	{
	}

	public GainGoldEachReward(int goldAmount)
	{
		_goldAmount = goldAmount;
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain {Icons.Inline(Icons.Coins, textParameters)}{_goldAmount} each.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		foreach(SavedCharacter savedCharacter in savedCampaign.Characters)
		{
			savedCharacter.AddGold(_goldAmount);
		}
	}
}