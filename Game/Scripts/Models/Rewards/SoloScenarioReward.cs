using System.Threading;
using Fractural.Tasks;
using Godot;

public class SoloScenarioReward : SavedReward
{
	private ItemModel[] _itemModels;

	public override RewardType Type => RewardType.Immediate;

	public SoloScenarioReward()
	{
	}

	public SoloScenarioReward(params ItemModel[] itemModels)
	{
		_itemModels = itemModels;
	}

	public override string GetLabelText(RichTextParameters textParameters)
	{
		string text = "";
		for(int i = 0; i < _itemModels.Length; i++)
		{
			if(i == 0)
			{
				text += "G";
			}
			else
			{
				text += "g";
			}

			text += $"ain 1 '{_itemModels[i].Name}' or ";
		}

		return text + "gain 1 perk mark.";
	}

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		switch(_itemModels.Length)
		{
			case 1:
				AppController.Instance.PopupManager.RequestPopup(new SoloScenarioRewardPopup.Request()
				{
					ItemModel = _itemModels[0],
					SavedCharacter = GameController.Instance.CharacterManager.Characters[0].SavedCharacter
				});
				break;
			case 2:
				AppController.Instance.PopupManager.RequestPopup(new SoloScenarioRewardTwoItemsPopup.Request()
				{
					ItemModel1 = _itemModels[0],
					ItemModel2 = _itemModels[1],
					SavedCharacter = GameController.Instance.CharacterManager.Characters[0].SavedCharacter
				});
				break;
			default:
				Log.Error("More than two item models in solo scenario reward! Not yet implemented.");
				break;
		}

		GameController.Instance.CharacterManager.Characters[0].SavedCharacter.SetSoloScenarioCompleted();

		await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<SoloScenarioRewardPopup.Request>(),
			cancellationToken: cancellationToken);
	}
}