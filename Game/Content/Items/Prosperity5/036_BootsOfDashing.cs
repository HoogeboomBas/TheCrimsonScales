using Fractural.Tasks;

public class BootsOfDashing : Prosperity5Item
{
	public override string Name => "Boots of Dashing";
	public override int ItemNumber => 36;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 0;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringMove(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AdjustMoveValue(3);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}