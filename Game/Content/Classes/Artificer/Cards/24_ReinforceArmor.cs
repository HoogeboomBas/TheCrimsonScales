using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ReinforceArmor : ArtificerCardModel<ReinforceArmor.CardTop, ReinforceArmor.CardBottom>
{
	public override string Name => "Reinforce Armor";
	public override int Level => 7;
	public override int Initiative => 26;
	protected override int AtlasIndex => 24;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.FromAttack && state.Performer.AlliedWith(parameters.Figure, true) &&
						              RangeHelper.Distance(state.Performer.Hex, parameters.Figure.Hex) <= 1,
						async parameters =>
						{
							parameters.AdjustShield(1);
							await GDTask.CompletedTask;
						});

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Figure, true) && RangeHelper.Distance(state.Performer.Hex, parameters.Figure.Hex) <= 1,
						applyParameters =>
						{
							applyParameters.AdjustShield(1);
						}
					);

					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure, true),
						async _ =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();

							await GDTask.CompletedTask;
						},
						EffectType.Visuals
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
			TimedTrack(
			[
				new UseSlot(new Vector2(0.29139274f, 0.3333333f)),
				new UseSlot(new Vector2(0.49925926f, 0.3333333f)),
				new UseSlot(new Vector2(0.7074074f, 0.3333333f))
			])
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 1);
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6211814f, 0.6303878f)))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(3)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.HealAfterTargetConfirmed.Subscription.New(
						applyFunction: async applyParameters =>
						{
							int currentHealth = applyParameters.AbilityState.Target.Health;
							applyParameters.AbilityState.SetCustomValue(this, "CurrentHealth", currentHealth);

							await GDTask.CompletedTask;
						})
				)
				.WithAfterHealPerformedSubscription(
					ScenarioEvents.AfterHealPerformed.Subscription.New(
						canApplyParameters => canApplyParameters.AbilityState.GetCustomValue<int>(this, "CurrentHealth") <
						                      canApplyParameters.AbilityState.Target.Health,
						async applyParameters =>
						{
							applyParameters.AbilityState.SetCustomValue(this, "IncreasedHealth", true);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainScrapToken(state);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					HealAbility.State healAbilityState = state.ActionState.GetAbilityState<HealAbility.State>(1);
					return healAbilityState.SingleTargetStates.Any(singleTargetState => singleTargetState.RemovedConditions.Count >= 1) ||
					       healAbilityState.GetCustomValue<bool>(this, "IncreasedHealth");
				})
				.Build())
		];
	}
}