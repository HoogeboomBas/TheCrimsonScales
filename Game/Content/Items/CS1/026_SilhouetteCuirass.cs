public class SilhouetteCuirass : CS1Item
{
	public override string Name => "Silhouette Cuirass";
	public override int ItemNumber => 26;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 45;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters => parameters.Figure == Owner && parameters.CalculatedCurrentDamage >= parameters.Figure.Health,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.SetDamagePrevented();
					await AbilityCmd.AddCondition(null, user, Conditions.Invisible, user);
				});
			}
		);
	}
}