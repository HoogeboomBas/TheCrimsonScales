using Fractural.Tasks;

public class IronSpear : JotLItem
{
	public override string Name => "Iron Spear";
	public override int ItemNumber => 10;
	public override int ShopCount => 2;
	public override int Cost => 15;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 2;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state =>
				state.Performer == Owner &&
				state.SingleTargetRangeType == RangeType.Melee &&
				state.IsSingleTarget &&
				state.SingleTargetRange == 1,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAdjustRange(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}