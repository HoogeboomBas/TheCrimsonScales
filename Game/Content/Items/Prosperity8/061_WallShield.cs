using Fractural.Tasks;

public class WallShield : Prosperity8Item
{
	public override string Name => "Wall Shield";
	public override int ItemNumber => 61;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 8;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters => parameters.FromAttack && parameters.Figure == Owner && parameters.WouldSufferDamage,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AdjustShield(4);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}