using Fractural.Tasks;

public class AllLoseItemScenarioEffect : ScenarioEffect
{
	private readonly ItemType _itemType;

	public AllLoseItemScenarioEffect(ItemType itemType)
	{
		_itemType = itemType;
	}

	public override string GetDescription(RichTextParameters textParameters) =>
		$"All characters {Icons.Inline(Icons.LoseCard, textParameters)} one {Icons.Inline(Icons.GetItem(_itemType), textParameters)} item.";

	public override async GDTask Activate(Character character)
	{
		ItemModel item = await AbilityCmd.SelectItem(character, ItemState.Available, _itemType,
			hintText: $"Select one {Icons.HintText(Icons.GetItem(_itemType))} to {Icons.HintText(Icons.LoseCard)}");

		if(item != null)
		{
			await item.SetItemState(ItemState.Consumed);
		}
	}
}