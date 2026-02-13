public class MoonEarring : Prosperity1Item
{
	public override string Name => "Moon Earring";
	public override int ItemNumber => 28;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 10;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					foreach(ItemModel item in user.Items)
					{
						if(item.ItemState == ItemState.Spent)
						{
							await AbilityCmd.RefreshItem(item);
						}
					}
				});
			}
		);
	}
}