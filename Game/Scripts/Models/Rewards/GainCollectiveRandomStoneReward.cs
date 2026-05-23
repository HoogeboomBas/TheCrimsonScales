using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainCollectiveRandomStoneReward : SavedReward
{
	public override RewardType Type => RewardType.Immediate;

	public GainCollectiveRandomStoneReward()
	{
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain one collective random “Stone” item.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		RandomNumberGenerator tempRNG = new RandomNumberGenerator();
		tempRNG.Randomize();

		ItemModel itemModel = AppController.GetRandomAvailableStone(savedCampaign, tempRNG);

		AppController.Instance.PopupManager.RequestPopup(new ItemRewardCharacterSelectionPopup.Request()
		{
			ItemModel = itemModel,
			Characters = savedCampaign.Characters,
			OnCharacterConfirmed = character =>
			{
				savedCampaign.GetSavedItem(itemModel).AddUnlocked(1);
				character.AddItem(itemModel);
			}
		});

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<ItemRewardCharacterSelectionPopup.Request>(),
			cancellationToken: cancellationToken);
	}
}