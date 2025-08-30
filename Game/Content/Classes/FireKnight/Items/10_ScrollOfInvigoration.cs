public class ScrollOfInvigoration : FireKnightItem
{
	public override string Name => "Scroll of Invigoration";
	public override int ItemNumber => 10;
	protected override int AtlasIndex => 10 - 10;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.InfuseElement(Element.Fire);

					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription moveSubscription =
						ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
							subscriptionParameters => true,
							async subscriptionParameters =>
							{
								ActionState actionState = new ActionState(user,
								[
									MoveAbility.Builder()
										.WithDistance(2)
										.WithMoveType(MoveType.Jump)
										.Build()
								]);
								await actionState.Perform();
							},
							effectType: EffectType.SelectableMandatory,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Move)}2, {Icons.Inline(Icons.Jump)}")
						);

					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription attackSubscription =
						ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
							subscriptionParameters => true,
							async subscriptionParameters =>
							{
								ActionState actionState = new ActionState(user,
								[
									AttackAbility.Builder()
										.WithDamage(2)
										.WithHasAdvantage(true)
										.Build()
								]);
								await actionState.Perform();
							},
							effectType: EffectType.SelectableMandatory,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}2, advantage")
						);

					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription healSubscription =
						ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
							subscriptionParameters => true,
							async subscriptionParameters =>
							{
								ActionState actionState = new ActionState(user,
								[
									HealAbility.Builder()
										.WithHealValue(2)
										.WithConditions(Conditions.Bless)
										.WithTarget(Target.Self)
										.Build()
								]);
								await actionState.Perform();
							},
							effectType: EffectType.SelectableMandatory,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"{Icons.Inline(Icons.Heal)}2 self, {Icons.Inline(Icons.GetCondition(Conditions.Bless))}")
						);

					await AbilityCmd.GenericChoice(user,
						[moveSubscription, attackSubscription, healSubscription], hintText: "Select the ability to perform");
				});
			}
		);
	}
}