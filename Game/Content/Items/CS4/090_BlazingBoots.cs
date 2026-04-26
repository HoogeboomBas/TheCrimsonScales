using Fractural.Tasks;

public class BlazingBoots : CS4Item
{
	public override string Name => "Blazing Boots";
	public override int ItemNumber => 90;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringMove(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AdjustMoveValue(state.MoveValue);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}