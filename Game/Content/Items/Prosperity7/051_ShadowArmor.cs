using Fractural.Tasks;

public class ShadowArmor : Prosperity7Item
{
	public override string Name => "Shadow Armor";
	public override int ItemNumber => 51;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 2;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters =>
				parameters.Figure == Owner &&
				parameters.WouldSufferDamage,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.SetDamagePrevented();

					await GDTask.CompletedTask;
				});
			}
		);
	}
}