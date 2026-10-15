using Fractural.Tasks;

public class MoonleatherBoots : CS2Item
{
	public override string Name => "Moonleather Boots";
	public override int ItemNumber => 40;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 13;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringMove(
			canApply: state => state.Performer == Owner && !Owner.IsDamaged(),
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AdjustMoveValue(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}