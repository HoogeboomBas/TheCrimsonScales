using Fractural.Tasks;

public class JetBoots : JotLItem
{
	public override string Name => "Jet Boots";
	public override int ItemNumber => 36;
	public override int ShopCount => 1;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 9;

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