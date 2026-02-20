using Fractural.Tasks;

public class LightweightBoots : CS1Item
{
	public override string Name => "Lightweight Boots";
	public override int ItemNumber => 17;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 29;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringMove(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AdjustMoveValue(2);
					state.AddJump();

					await GDTask.CompletedTask;
				});
			}
		);
	}
}