using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainCollectiveItemReward : SavedReward
{
	[JsonProperty]
	private string _itemModelId;

	private ItemModel ItemModel => ModelDB.GetById<ItemModel>(_itemModelId);

	public override RewardType Type => RewardType.Immediate;

	public GainCollectiveItemReward()
	{
	}

	public GainCollectiveItemReward(ItemModel itemModel)
	{
		_itemModelId = itemModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain 1 collective '{ItemModel.Name}'.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		AppController.Instance.PopupManager.RequestPopup(new ItemRewardCharacterSelectionPopup.Request()
		{
			ItemModel = ItemModel,
			Characters = savedCampaign.Characters,
			OnCharacterConfirmed = character =>
			{
				savedCampaign.GetSavedItem(ItemModel).AddUnlocked(1);
				character.AddItem(ItemModel);
			}
		});

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<ItemRewardCharacterSelectionPopup.Request>(),
			cancellationToken: cancellationToken);
	}
}