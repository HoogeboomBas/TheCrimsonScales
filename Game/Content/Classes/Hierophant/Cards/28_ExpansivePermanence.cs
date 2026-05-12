using System.Collections.Generic;
using Fractural.Tasks;

public class ExpansivePermanence : HierophantLevelUpCardModel<ExpansivePermanence.CardTop, ExpansivePermanence.CardBottom>
{
	public override string Name => "Expansive Permanence";
	public override int Level => 9;
	public override int Initiative => 09;
	protected override int AtlasIndex => 15 - 14;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
					[
						ShieldAbility.Builder()
							.WithShieldValue(2)
							.WithAbilityStartedSubscription(
								ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Earth,
									canApplyParameters => true,
									async applyParameters =>
									{
										((ShieldAbility.State)applyParameters.AbilityState).AdjustAdditionalShield(1);

										await AbilityCmd.AddCondition(null, applyParameters.Performer, Conditions.Immobilize);
										await AbilityCmd.GainXP(grantAbilityState.Performer, 1);
									},
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										$"+1{Icons.Inline(Icons.Shield)}, gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}"))
							)
							.WithOnAbilityEndedPerformed(async shieldAbilityState =>
							{
								ScenarioEvents.AbilityStartedEvent.Subscribe(shieldAbilityState, this,
									parameters => parameters.Performer == shieldAbilityState.Performer &&
									              parameters.AbilityState is AttackAbility.State attackAbilityState &&
									              attackAbilityState.IsSingleTarget,
									async parameters =>
									{
										((AttackAbility.State)parameters.AbilityState).AbilityAdjustRange(100);
										await GDTask.CompletedTask;
									});
								ScenarioEvents.RoundEndedEvent.Subscribe(shieldAbilityState, this,
									parameters => true,
									async parameters =>
									{
										ScenarioEvents.AbilityStartedEvent.Unsubscribe(shieldAbilityState, this);
										ScenarioEvents.RoundEndedEvent.Unsubscribe(shieldAbilityState, this);
										await GDTask.CompletedTask;
									});
								await GDTask.CompletedTask;
							})
							.Build(),
						OtherActiveAbility.Builder()
							.WithOnActivate(async state =>
							{
								ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
									parameters =>
										parameters.Performer == state.Performer &&
										parameters.AbilityState is AttackAbility.State attackAbilityState &&
										attackAbilityState.IsSingleTarget,
									async parameters =>
									{
										((AttackAbility.State)parameters.AbilityState).AbilityAdjustRange(100);
										await GDTask.CompletedTask;
									});
								await GDTask.CompletedTask;
							})
							.WithOnDeactivate(async state =>
							{
								ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
								await GDTask.CompletedTask;
							})
							.Build()
					]
				)
				.WithRange(3)
				.WithTarget(Target.SelfOrAllies)
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState is AttackAbility.State &&
						              (parameters.AbilityState.Performer.AlliedWith(state.Performer) ||
						               parameters.AbilityState.Performer.EnemiesWith(state.Performer)),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilityAdjustAttackValue(
								parameters.AbilityState.Performer.AlliedWith(state.Performer) ? 2 : -2);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
		public override int XP => 2;
		public override bool Round => true;
		public override bool Loss => true;
	}
}