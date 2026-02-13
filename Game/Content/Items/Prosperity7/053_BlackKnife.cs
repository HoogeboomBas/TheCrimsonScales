using Fractural.Tasks;

public class BlackKnife : Prosperity7Item
{
	public override string Name => "Black Knife";
	public override int ItemNumber => 53;
	public override int ShopCount => 2;
	public override int Cost => 25;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
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