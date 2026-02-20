public class SuperHealingPotion : Prosperity7Item
{
	public override string Name => "Super Healing Potion";
	public override int ItemNumber => 55;
	public override int ShopCount => 2;
	public override int Cost => 50;
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
					ActionState actionState = new ActionState(user,
					[
						HealAbility.Builder()
							.WithHealValue(7)
							.WithTarget(Target.Self).Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}