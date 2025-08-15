using Fractural.Tasks;

public class WingedShoes : Prosperity1Item
{
	public override string Name => "Winged Shoes";
	public override int ItemNumber => 2;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 2;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringMove(
			canApply: state => state.Performer == Owner && state.MoveType == MoveType.Regular,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AddJump();
					//state.AdjustMoveType(MoveType.Jump);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}