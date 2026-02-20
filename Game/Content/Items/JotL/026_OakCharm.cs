public class OakCharm : JotLItem
{
	public override string Name => "Oak Charm";
	public override int ItemNumber => 26;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

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
					ActionState actionState = new ActionState(user,
						[
							ConditionAbility.Builder().WithConditions(Conditions.Bless).WithRange(5).Build()
						]
					);
					await actionState.Perform();
				});
			}
		);
	}
}