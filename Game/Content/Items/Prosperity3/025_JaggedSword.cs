using Fractural.Tasks;

public class JaggedSword : Prosperity3Item
{
	public override string Name => "Jagged Sword";
	public override int ItemNumber => 25;
	public override int ShopCount => 2;
	public override int Cost => 30;
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
					state.SingleTargetAddCondition(Conditions.Wound1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}