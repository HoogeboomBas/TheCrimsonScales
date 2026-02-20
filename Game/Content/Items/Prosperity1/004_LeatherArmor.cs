using Fractural.Tasks;

public class LeatherArmor : Prosperity1Item
{
	public override string Name => "Leather Armor";
	public override int ItemNumber => 4;
	public override int ShopCount => 2;
	public override int Cost => 15;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAttackAfterTargetConfirmed(
			canApply: state => state.Target == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetSetHasDisadvantage();

					await GDTask.CompletedTask;
				});
			}
		);
	}
}