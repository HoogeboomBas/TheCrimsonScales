using Fractural.Tasks;

public class RocketBoots : GHRewardsItem
{
	public override string Name => "Winged Shoes";
	public override int ItemNumber => 96;
	public override int ShopCount => 2;
	public override int Cost => 80;
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
					state.AddJump();

					await GDTask.CompletedTask;
				});
			}
		);
	}
}