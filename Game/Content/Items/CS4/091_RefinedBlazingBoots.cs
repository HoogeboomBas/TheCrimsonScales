using Fractural.Tasks;

public class RefinedBlazingBoots : CS4Item
{
	public override string Name => "Refined Blazing Boots";
	public override int ItemNumber => 91;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 7;

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
					state.AdjustMoveType(MoveType.Jump);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}