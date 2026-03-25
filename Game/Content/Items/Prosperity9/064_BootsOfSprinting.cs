using Fractural.Tasks;

public class BootsOfSprinting : Prosperity9Item
{
	public override string Name => "Boots of Sprinting";
	public override int ItemNumber => 64;
	public override int ShopCount => 2;
	public override int Cost => 75;
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
					state.AdjustMoveValue(4);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}