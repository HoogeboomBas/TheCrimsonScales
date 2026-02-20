public class HookShot : CS2Item
{
	public override string Name => "Hook Shot";
	public override int ItemNumber => 39;
	public override int ShopCount => 1;
	public override int Cost => 45;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 12;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(user, [AttackAbility.Builder().WithDamage(2).WithRange(3).WithPierce(2).Build()]);
					await actionState.Perform();
				});
			}
		);
	}
}