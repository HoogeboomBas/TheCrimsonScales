using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AllStartScenarioSpendingItemTypeReward : SavedReward
{
	[JsonProperty]
	private ItemType _itemType;

	public AllStartScenarioSpendingItemTypeReward()
	{
	}

	public AllStartScenarioSpendingItemTypeReward(ItemType itemType)
	{
		_itemType = itemType;
	}

	public override RewardType Type => RewardType.ScenarioStart;

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"All characters start the next scenario spending a {Icons.Inline(Icons.GetItem(_itemType), textParameters)} each.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			ItemModel item =
				await AbilityCmd.SelectItem(character, ItemState.Spent, requiredItemType: _itemType, hintText: "Select an item to spend");
			if(item != null)
			{
				await AbilityCmd.SpendItem(item);
			}
		}
	}
}