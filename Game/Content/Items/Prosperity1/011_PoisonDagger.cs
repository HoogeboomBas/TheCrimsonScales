using Fractural.Tasks;

public class PoisonDagger : Prosperity1Item
{
	public override string Name => "Poison Dagger";
	public override int ItemNumber => 11;
	public override int ShopCount => 2;
	public override int Cost => 15;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 20;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAddCondition(Conditions.Poison1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}