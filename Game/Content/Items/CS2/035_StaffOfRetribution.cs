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
			canApply: parameters =>
				parameters.RetaliatingFigure == Owner &&
				RangeHelper.Distance(parameters.AbilityState.Performer.Hex, parameters.RetaliatingFigure.Hex) <= 3 + parameters.ExtraRange,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AddRetaliate(1, 3);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}