using Fractural.Tasks;
using Godot;

public class GainCollectiveItemEventReward(ItemModel itemModel) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Gain 1 collective '{itemModel.Name}'.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		AppController.Instance.PopupManager.RequestPopup(new ItemRewardCharacterSelectionPopup.Request()
		{
			ItemModel = itemModel,
			Characters = BetweenScenariosController.Instance.SavedCampaign.Characters,
			OnCharacterConfirmed = character =>
			{
				BetweenScenariosController.Instance.SavedCampaign.GetSavedItem(itemModel).AddUnlocked(1);
				character.AddItem(itemModel);
			}
		});

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<ItemRewardCharacterSelectionPopup.Request>());
	}
}