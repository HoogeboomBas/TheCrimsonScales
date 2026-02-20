using Fractural.Tasks;

public class BlackCandle : JotLItem
{
	public override string Name => "Black Candle";
	public override int ItemNumber => 19;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 3;

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
					state.SingleTargetAddCondition(Conditions.Curse);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}