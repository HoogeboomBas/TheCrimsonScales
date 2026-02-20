public class IntoxicatingPotion : CS1Item
{
	public override string Name => "Intoxicating Potion";
	public override int ItemNumber => 29;
	public override int ShopCount => 1;
	public override int Cost => 15;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 49;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(user, [
						HealAbility.Builder().WithHealValue(5)
							.WithConditions(Conditions.Poison1).WithTarget(Target.Self).Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}