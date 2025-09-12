public class AmuletOfLife : Prosperity3Item
{
	public override string Name => "Amulet of Life";
	public override int ItemNumber => 24;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(character, [HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()]);
					await actionState.Perform();
				});
			}
		);
	}
}