using System.Linq;
using Fractural.Tasks;
using Godot;

public class LoseCollectiveGoldEventReward(int goldAmount) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Lose {goldAmount} collective {Icons.Inline(Icons.Coins, color: textColor)}.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		int adjustedGoldAmount = Mathf.Min(goldAmount, BetweenScenariosController.Instance.SavedCampaign.Characters.Sum(character => character.Gold));
		AppController.Instance.PopupManager.RequestPopup(new GoldDistributionPopup.Request()
		{
			Gold = adjustedGoldAmount,
			LoseGold = true,
			Characters = BetweenScenariosController.Instance.SavedCampaign.Characters,
		});

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<GoldDistributionPopup.Request>());
	}
}