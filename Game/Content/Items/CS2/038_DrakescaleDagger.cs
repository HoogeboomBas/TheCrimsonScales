using Fractural.Tasks;

public class DrakescaleDagger : CS2Item
{
	public override string Name => "Drakescale Dagger";
	public override int ItemNumber => 38;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 11;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetSetDrawAMDCard(false);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}