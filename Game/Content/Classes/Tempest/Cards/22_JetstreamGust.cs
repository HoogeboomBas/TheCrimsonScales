using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class JetstreamGust : TempestCardModel<JetstreamGust.CardTop, JetstreamGust.CardBottom>
{
	public override string Name => "Jetstream Gust";
	public override int Level => 6;
	public override int Initiative => 69;
	protected override int AtlasIndex => 22;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.44501105f, 0.18695761f)))
				.WithPush(3, new PushSquare(this, new Vector2(0.66594267f, 0.1865036f)))
				.WithDuringAttackSubscription(
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "Wound", true);
							await GDTask.CompletedTask;
						},
						new TextEffectInfoView.Parameters(
							$"All enemies pushed through also gain {Icons.Inline(Icons.GetCondition(Conditions.Wound1))}")))
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(applyFunction: async parameters =>
					{
						foreach(Figure figure in parameters.AbilityState.SingleTargetState.PushHexes.SelectMany(hex => hex.GetFigures())
							        .Where(figure => figure != parameters.AbilityState.Target))
						{
							await AbilityCmd.SufferDamage(parameters.AbilityState, figure, 1);
							await AbilityCmd.AddConditions(parameters.AbilityState, figure,
								parameters.AbilityState.GetCustomValue<bool>(this, "Wound")
									? [Conditions.Wound1, Conditions.Muddle]
									: [Conditions.Muddle]);
						}
					}))
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.5245223f, 0.61284244f)))
				.WithMoveType(MoveType.Jump)
				.WithAbilityPerformedSubscription(
					ScenarioEvents.AbilityPerformed.Subscription.New(parameters => AbilityCmd.HexesInStraightLine(
							((MoveAbility.State)parameters.AbilityState).Origin,
							((MoveAbility.State)parameters.AbilityState).Hexes, 3),
						async parameters =>
						{
							await GainSpark(parameters.AbilityState);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state =>
					LoseSparksConsumeElementConditionalAbilityCheck(state.Performer, 1, Element.Air,
						$"{Icons.Inline(Icons.GetCondition(Conditions.Dodge))}, self"))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditionalAbilityCheck(state =>
					LoseSparksConditionalAbilityCheck(state.Performer, 2, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}2")))
				.Build())
		];
	}
}