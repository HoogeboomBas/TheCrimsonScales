using Fractural.Tasks;

public class DrakescaleShield : CS2Item
{
	public override string Name => "Drakescale Shield";
	public override int ItemNumber => 37;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 9;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeRetaliate(
			canApply: state =>
				state.RetaliatingFigure == Owner &&
				RangeHelper.Distance(state.AbilityState.Performer.Hex, state.RetaliatingFigure.Hex) <= 1,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AdjustRetaliate(3);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}