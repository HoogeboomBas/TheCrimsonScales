public class RingOfStrength : JotLItem
{
	public override string Name => "Ring of Strength";
	public override int ItemNumber => 31;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 6;

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
						ConditionAbility.Builder()
							.WithConditions(Conditions.Strengthen)
							.WithTarget(Target.Self)
							.Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}