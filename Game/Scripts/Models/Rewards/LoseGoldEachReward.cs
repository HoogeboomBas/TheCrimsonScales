using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class LoseGoldEachReward : SavedReward
{
	[JsonProperty]
	private int _goldAmount;

	public LoseGoldEachReward()
	{
	}

	public LoseGoldEachReward(int goldAmount)
	{
		_goldAmount = goldAmount;
	}

	public override RewardType Type => RewardType.Immediate;
	public override string GetLabelText(RichTextParameters textParameters) => $"Lose {Icons.Inline(Icons.Coins, textParameters)}{_goldAmount} each.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		foreach(SavedCharacter savedCharacter in savedCampaign.Characters)
		{
			savedCharacter.RemoveGold(Mathf.Min(savedCharacter.Gold, _goldAmount));
		}
	}
}