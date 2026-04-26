public class BulwarkBanner : CS4Item
{
	public override string Name => "Bulwark Banner";
	public override int ItemNumber => 86;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool Round => true;

	protected override int AtlasIndex => 2;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await GetActionState(user,
					[
						GrantAbility.Builder()
							.WithAbilities(ShieldAbility.Builder().WithShieldValue(1).Build())
							.WithInfiniteRange()
							.WithRequiresLineOfSight(false)
							.WithTarget(Target.SelfOrAllies | Target.TargetAll)
							.Build()
					]).Perform();
				});
			}
		);
	}
}