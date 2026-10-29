using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SwiftStrike : TempestCardModel<SwiftStrike.CardTop, SwiftStrike.CardBottom>
{
	public override string Name => "Swift Strike";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 9;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.29355386f, 0.17728531f)))
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithDuringAttackSubscription(
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(3);
							parameters.AbilityState.AbilitySetHasAdvantage();

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Air);
							await AbilityCmd.GainXP(parameters.Performer, 1);

							parameters.AbilityState.ActionState.SetOverrideLoss();
						},
						new TextEffectInfoView.Parameters(
							$"+3{Icons.Inline(Icons.Attack)}, advantage, {Icons.Inline(Icons.GetElement(Element.Air))}, {Icons.Inline(Icons.LoseCard)}")))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Dodge);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<AttackAbility.State>(0).KilledTargets.Count > 0;
				})
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6202788f, 0.7036012f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state =>
					LoseSparksConsumeElementConditionalAbilityCheck(state.Performer, 1, Element.Air,
						$"{Icons.Inline(Icons.GetCondition(Conditions.Dodge))}, self"))
				.Build())
		];
	}
}