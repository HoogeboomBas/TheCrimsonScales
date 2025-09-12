using System.Linq;

public class EmpoweringTalisman : Prosperity2Item
{
	public override string Name => "Empowering Talisman";
	public override int ItemNumber => 17;
	public override int ShopCount => 2;
	public override int Cost => 45;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character =>
				character == Owner &&
				character.Items.Any(item => item.ItemState == ItemState.Consumed && item.ItemType == ItemType.Small),
			apply: async character =>
			{
				await Use(async user =>
				{
					ItemModel item = await AbilityCmd.SelectItem(character, ItemState.Consumed, ItemType.Small, "Select an item to refresh");
					await AbilityCmd.RefreshItem(item);
				});
			}
		);
	}
}