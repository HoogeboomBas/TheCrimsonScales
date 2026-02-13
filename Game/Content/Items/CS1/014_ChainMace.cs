public class ChainMace : CS1Item
{
	public override string Name => "Chain Mace";
	public override int ItemNumber => 14;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 24;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(user, [AttackAbility.Builder().WithDamage(3).WithRange(2).Build()]);
					await actionState.Perform();
				});
			}
		);
	}
}