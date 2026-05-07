using System;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class LoseCollectiveGoldReward : SavedReward
{
	private readonly int _goldAmount;

	public override RewardType Type => RewardType.Immediate;

	public LoseCollectiveGoldReward()
	{
	}

	public LoseCollectiveGoldReward(int goldAmount)
	{
		_goldAmount = goldAmount;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"Lose {_goldAmount} collective {Icons.Inline(Icons.Coins, textParameters)}.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		int adjustedGoldAmount = Mathf.Min(_goldAmount, savedCampaign.Characters.Sum(character => character.Gold));
		AppController.Instance.PopupManager.RequestPopup(new GoldDistributionPopup.Request()
		{
			Gold = adjustedGoldAmount,
			LoseGold = true,
			Characters = savedCampaign.Characters,
		});

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<GoldDistributionPopup.Request>(),
			cancellationToken: cancellationToken);
	}
}