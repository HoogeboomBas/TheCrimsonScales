using Fractural.Tasks;
using Godot;

public class GainCollectiveGoldEventReward(int goldAmount) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Gain {goldAmount} collective {Icons.Inline(Icons.Coins, color: textColor)}.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		AppController.Instance.PopupManager.RequestPopup(new GoldDistributionPopup.Request()
		{
			Gold = goldAmount,
			LoseGold = false,
			Characters = BetweenScenariosController.Instance.SavedCampaign.Characters,
		});

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<GoldDistributionPopup.Request>());
	}
}