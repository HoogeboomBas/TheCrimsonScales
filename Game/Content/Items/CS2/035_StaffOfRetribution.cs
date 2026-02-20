using Fractural.Tasks;

public class StaffOfRetribution : CS2Item
{
	public override string Name => "Staff of Retribution";
	public override int ItemNumber => 35;
	public override int ShopCount => 1;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 7;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeRetaliate(
			canApply: state =>
				state.RetaliatingFigure == Owner &&
				RangeHelper.Distance(state.AbilityState.Performer.Hex, state.RetaliatingFigure.Hex) <= 3,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AdjustRetaliate(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}