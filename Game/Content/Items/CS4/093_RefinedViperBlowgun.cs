public class RefinedViperBlowgun : CS4Item
{
	public override string Name => "Refined Viper Blowgun";
	public override int ItemNumber => 93;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 9;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ActionState actionState = GetActionState(user, [
						AttackAbility.Builder()
							.WithDamage(1)
							.WithConditions(Conditions.Poison1)
							.WithRange(4)
							.WithDuringAttackSubscription(
								ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
									applyFunction: async parameters =>
									{
										parameters.AbilityState.AdjustTargets(1);
									},
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}")
								)
							)
							.Build()
					]);
					await actionState.Perform();
				});
			}
		);
	}
}