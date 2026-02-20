using Fractural.Tasks;

public class VipertoothDagger : CS2Item
{
	public override string Name => "Vipertooth Dagger";
	public override int ItemNumber => 54;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 27;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee &&
			                   state.Performer.Hex.HasHexObjectOfType<DifficultTerrain>(),
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetSetHasAdvantage();

					await GDTask.CompletedTask;
				});
			}
		);
	}
}