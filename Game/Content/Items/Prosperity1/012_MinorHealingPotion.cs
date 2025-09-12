public class MinorHealingPotion : Prosperity1Item
{
	public override string Name => "Minor Healing Potion";
	public override int ItemNumber => 12;
	public override int ShopCount => 2;
	public override int Cost => 10;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 22;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(character, [HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build()]);
					await actionState.Perform();
				});
			}
		);
	}
}