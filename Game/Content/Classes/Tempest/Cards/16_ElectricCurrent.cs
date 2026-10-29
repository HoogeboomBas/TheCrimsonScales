using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ElectricCurrent : TempestCardModel<ElectricCurrent.CardTop, ElectricCurrent.CardBottom>
{
	public override string Name => "Electric Current";
	public override int Level => 3;
	public override int Initiative => 20;
	protected override int AtlasIndex => 16;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.50907683f, 0.13558808f)))
				.WithConditions(Conditions.Immobilize)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => LoseSparksConditionalAbilityCheck(state.Performer, 1,
					new TextEffectInfoView.Parameters($"One enemy within {Icons.Inline(Icons.Range)}2 suffers {Icons.Inline(Icons.Damage)}1")))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.5238462f, 0.667036f)))
				.WithMoveType(MoveType.Jump)
				.WithAbilityPerformedSubscriptions(
				[
					ScenarioEvents.AbilityPerformed.Subscription.New(
						parameters => ((MoveAbility.State)parameters.AbilityState).Hexes
							.SelectMany(hex => hex.GetFigures()).Any(figure => parameters.Performer.EnemiesWith(figure)),
						async parameters =>
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Air);
							await GainSpark(parameters.AbilityState);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}),
					LoseSparksSubscription<ScenarioEvents.AbilityPerformed.Parameters>(2, async parameters =>
					{
						foreach(Figure figure in ((MoveAbility.State)parameters.AbilityState).Hexes.SelectMany(hex =>
							        hex.GetFigures().Where(figure => parameters.Performer.EnemiesWith(figure)))
						        .Distinct())
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Immobilize);
						}
					}, new TextEffectInfoView.Parameters($"All enemies moved through gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}"))
				])
				.Build()),
		];
	}
}