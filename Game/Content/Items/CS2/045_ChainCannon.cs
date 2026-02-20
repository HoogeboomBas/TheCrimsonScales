public class ChainCannon : CS2Item
{
	public override string Name => "Chain Cannon";
	public override int ItemNumber => 45;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 18;

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
						PullSelfAbility.Builder().WithPullSelfValue(2).WithRange(3).Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}