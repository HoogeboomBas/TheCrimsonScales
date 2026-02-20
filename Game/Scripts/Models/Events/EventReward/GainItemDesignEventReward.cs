using Fractural.Tasks;
using Godot;

public class GainItemDesignEventReward(ItemModel itemModel) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Gain '{itemModel.Name}' item design.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		SavedItem savedItem = BetweenScenariosController.Instance.SavedCampaign.GetSavedItem(itemModel);
		savedItem.AddUnlocked(1);
		savedItem.AddStock(1);
	}
}