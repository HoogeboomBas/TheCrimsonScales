using Fractural.Tasks;

public class GhostViperVenom : CS2Item
{
	public override string Name => "Ghost Viper Venom";
	public override int ItemNumber => 55;
	public override int ShopCount => 2;
	public override int Cost => 25;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 28;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAddCondition(Conditions.Poison2);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}