using Fractural.Tasks;

public class TowerShield : Prosperity4Item
{
	public override string Name => "Tower Shield";
	public override int ItemNumber => 29;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters => parameters.FromAttack && parameters.Figure == Owner && parameters.WouldSufferDamage,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AdjustShield(2);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}