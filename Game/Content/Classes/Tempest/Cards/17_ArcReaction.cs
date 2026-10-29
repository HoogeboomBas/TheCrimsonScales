using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ArcReaction : TempestCardModel<ArcReaction.CardTop, ArcReaction.CardBottom>
{
	public override string Name => "Arc Reaction";
	public override int Level => 4;
	public override int Initiative => 33;
	protected override int AtlasIndex => 17;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(new DynamicInt<AttackAbility.State>(state => SparkCount(state.Performer) + 1))
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithPierce(1)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.Consume([Element.Air, Element.Light])],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
							parameters.AbilityState.ActionState.SetOverrideLoss();
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Targets)}1, {Icons.Inline(Icons.LoseCard)}")))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					LoseSparks(state.Performer);
					state.SetPerformed();

					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62163085f, 0.65927976f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Strengthen, Conditions.Bless])
				.WithTarget(Target.Allies)
				.WithRange(2)
				.WithAfterTargetConfirmedSubscription(
					LoseSparksSubscription<ScenarioEvents.ConditionAfterTargetConfirmed.Parameters>(3, async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);
							parameters.AbilityState.AdjustTarget(Target.Self);
							Action<TargetedAbilityState, List<Figure>> oldCustomGetTargets = parameters.AbilityState.AbilityCustomGetTargets;
							parameters.AbilityState.SetAbilityCustomTargets((state, figures) =>
							{
								figures.AddRange(RangeHelper.GetFiguresInRange(state.Target, 2));
								parameters.AbilityState.SetAbilityCustomTargets(oldCustomGetTargets);
							});

							await GDTask.CompletedTask;
						},
						new TextEffectInfoView.Parameters(
							$"Also {Icons.Inline(Icons.Targets)}1 ally or self within {Icons.Inline(Icons.Range)}2 of the target")))
				.Build())
		];
	}
}