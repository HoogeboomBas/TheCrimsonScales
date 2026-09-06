using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SoloScenarioReward : SavedReward
{
	[JsonProperty]
	private string _itemModelId;

	private ItemModel ItemModel => ModelDB.GetById<ItemModel>(_itemModelId);

	public override RewardType Type => RewardType.Immediate;

	public SoloScenarioReward()
	{
	}

	public SoloScenarioReward(ItemModel itemModel)
	{
		_itemModelId = itemModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain 1 '{ItemModel.Name}' or gain 1 perk mark.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		AppController.Instance.PopupManager.RequestPopup(new SoloScenarioRewardPopup.Request()
		{
			ItemModel = ItemModel,
			SavedCharacter = GameController.Instance.CharacterManager.Characters[0].SavedCharacter
		});

		GameController.Instance.CharacterManager.Characters[0].SavedCharacter.SetSoloScenarioCompleted();

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<SoloScenarioRewardPopup.Request>(),
			cancellationToken: cancellationToken);
	}
}