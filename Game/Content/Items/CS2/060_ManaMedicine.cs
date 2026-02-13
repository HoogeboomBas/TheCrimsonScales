public class ManaMedicine : CS2Item
{
	public override string Name => "Mana Medicine";
	public override int ItemNumber => 50;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 36;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = new ActionState(user, [HealAbility.Builder().WithHealValue(5).WithTarget(Target.Self).Build()]);
					await actionState.Perform();
					await AbilityCmd.InfuseWildElement(null, character);
				});
			}
		);
	}
}