public class SunEarring : Prosperity5Item
{
	public override string Name => "Sun Earring";
	public override int ItemNumber => 49;
	public override int ShopCount => 2;
	public override int Cost => 35;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 14;

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

					ActionState actionState = new ActionState(user,
					[
						HealAbility.Builder()
							.WithHealValue(3)
							.WithTarget(Target.Self).Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}