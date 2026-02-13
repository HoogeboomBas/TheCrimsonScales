using Fractural.Tasks;

public class BoosterShot : CS2Item
{
	public override string Name => "Booster Shot";
	public override int ItemNumber => 49;
	public override int ShopCount => 1;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 22;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription recoverSubscription =
						ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
							subscriptionParameters => true,
							async subscriptionParameters =>
							{
								AbilityCard card = await AbilityCmd.SelectAbilityCard(character, CardState.Discarded,
									hintText: $"Select a discarded card to recover");

								if(card != null)
								{
									await AbilityCmd.ReturnToHand(card);
								}
							},
							effectType: EffectType.SelectableMandatory,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.RecoverCard),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.RecoverCard)} one discarded card")
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

					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription attackSubscription =
						ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
							subscriptionParameters => true,
							async subscriptionParameters =>
							{
								ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this, _subscriber,
									parameters => parameters.Performer == Owner,
									async parameters =>
									{
										ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(this, _subscriber);

										parameters.AbilityState.SingleTargetAdjustAttackValue(2);

										await GDTask.CompletedTask;
									}
								);

								await GDTask.CompletedTask;
							},
							effectType: EffectType.SelectableMandatory,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Attack),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}2, advantage")
						);

					await AbilityCmd.GenericChoice(user,
						[recoverSubscription, healSubscription, attackSubscription], hintText: "Select the ability to perform");
				});
			}
		);
	}
}