using Fractural.Tasks;

public class WeightedNet : Prosperity2Item
{
	public override string Name => "Weighted Net";
	public override int ItemNumber => 19;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 8;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Range,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAddCondition(Conditions.Immobilize);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}