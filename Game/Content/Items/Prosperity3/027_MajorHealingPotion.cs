public class MajorHealingPotion : Prosperity3Item
{
	public override string Name => "Major Healing Potion";
	public override int ItemNumber => 27;
	public override int ShopCount => 2;
	public override int Cost => 30;
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
							.WithHealValue(5)
							.WithTarget(Target.Self).Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}