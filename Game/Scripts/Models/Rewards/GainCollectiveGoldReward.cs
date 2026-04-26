using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainCollectiveGoldReward : SavedReward
{
	[JsonProperty]
	private int _goldAmount;

	public override RewardType Type => RewardType.Immediate;

	public GainCollectiveGoldReward()
	{
	}

	public GainCollectiveGoldReward(int goldAmount)
	{
		_goldAmount = goldAmount;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"Gain {_goldAmount} collective {Icons.Inline(Icons.Coins, textParameters)}.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		AppController.Instance.PopupManager.RequestPopup(new GoldDistributionPopup.Request()
		{
			Gold = _goldAmount,
			LoseGold = false,
			Characters = savedCampaign.Characters,
		});

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<GoldDistributionPopup.Request>(),
			cancellationToken: cancellationToken);
	}
}