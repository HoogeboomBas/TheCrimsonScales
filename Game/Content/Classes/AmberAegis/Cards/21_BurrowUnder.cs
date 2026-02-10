using System.Collections.Generic;
using System.Linq;
using Godot;

public class BurrowUnder : AmberAegisCardModel<BurrowUnder.CardTop, BurrowUnder.CardBottom>
{
	public override string Name => "Burrow Under";
	public override int Level => 5;
	public override int Initiative => 22;
	protected override int AtlasIndex => 21;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.5244444f, 0.17407405f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.28f, 0.28518516f)))
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes
						.SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeChoiceElement([Element.Fire, Element.Earth],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}")))
				.Build())
		];
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTarget(Target.Allies | Target.Enemies)
				.WithTargets(2)
				.WithRange(3)
				.WithFilterTargets((state, figure) =>
				{
					if(state.UniqueTargetedFigures.Any(target => target.EnemiesWith(state.Performer)) && figure.EnemiesWith(state.Performer))
					{
						return false;
					}

					if(state.UniqueTargetedFigures.Any(target => target.AlliedWith(state.Performer)) && figure.AlliedWith(state.Performer))
					{
						return false;
					}

					return true;
				})
				.WithAbilityPerformedSubscription(
					ScenarioEvents.AbilityPerformed.Subscription.New(
						parameters => ((ConditionAbility.State)parameters.AbilityState).UniqueTargetedFigures.Any(figure =>
							figure.AlliedWith(parameters.AbilityState.Performer)),
						async parameters =>
						{
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
				.WithTarget(Target.Any | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<ConditionAbility.State>(0).UniqueTargetedFigures);
				})
				.WithMandatory(true)
				.Build())
		];
	}
}