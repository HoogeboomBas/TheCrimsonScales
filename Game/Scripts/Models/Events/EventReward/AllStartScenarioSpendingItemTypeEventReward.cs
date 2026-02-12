using Fractural.Tasks;
using Godot;

public class AllStartScenarioSpendingItemTypeEventReward(ItemType itemType) : EventReward
{
	public override EventRewardType Type => EventRewardType.ScenarioStart;

	public override string GetLabelText(Color textColor) =>
		$"All characters start the next scenario spending a {Icons.Inline(Icons.GetItem(itemType), color: textColor)} each.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			ItemModel item =
				await AbilityCmd.SelectItem(character, ItemState.Spent, requiredItemType: itemType, hintText: "Select an item to spend");
			if(item != null)
			{
				await AbilityCmd.SpendItem(item);
			}
		}
	}
}