using Fractural.Tasks;

public class ThrowingHammer : JotLItem
{
	public override string Name => "Throwing Hammer";
	public override int ItemNumber => 8;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 1;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state =>
				state.Performer == Owner &&
				state.SingleTargetRangeType == RangeType.Range,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAddCondition(Conditions.Stun);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}