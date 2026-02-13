using Fractural.Tasks;

public class HeaterShield : Prosperity1Item
{
	public override string Name => "Heater Shield";
	public override int ItemNumber => 8;
	public override int ShopCount => 2;
	public override int Cost => 15;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 14;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters => parameters.FromAttack && parameters.Figure == Owner && parameters.WouldSufferDamage,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AdjustShield(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}