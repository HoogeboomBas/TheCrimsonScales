using Fractural.Tasks;

public class CanisterProjectile : CS2Item
{
	public override string Name => "Canister Projectile";
	public override int ItemNumber => 44;
	public override int ShopCount => 1;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 17;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Range,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAdjustPierce(2);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}