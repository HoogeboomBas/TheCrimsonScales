using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class StormLightning : TempestCardModel<StormLightning.CardTop, StormLightning.CardBottom>
{
	public override string Name => "Storm Lightning";
	public override int Level => 9;
	public override int Initiative => 88;
	protected override int AtlasIndex => 27;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithPierce(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red)
					]
				))
				.WithDuringAttackSubscription(
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(2);
							parameters.AbilityState.AbilityAdjustPierce(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);

							parameters.AbilityState.ActionState.SetOverrideLoss();
						},
						new TextEffectInfoView.Parameters(
							$"+2{Icons.Inline(Icons.Attack)}, +2{Icons.Inline(Icons.Pierce)}, {Icons.Inline(Icons.LoseCard)}")))
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure figure in state.GetRedAOEHexes().SelectMany(hex => RangeHelper.GetFiguresInRange(hex, 1, false)).Distinct()
						        .Where(figure =>
							        !figure.Hexes.Union(state.GetRedAOEHexes()).Any() &&
							        (state.Performer.AlliedWith(figure) || state.Performer.EnemiesWith(figure))))
					{
						await AbilityCmd.SufferDamage(state, figure, 2);
					}
				})
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && (parameters.AbilityState.Target.HasWound() ||
						                                                          parameters.AbilityState.Target.HasCondition(Conditions.Immobilize)),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await GDTask.CompletedTask;
						});

					await AbilityCmd.GainXP(state.Performer, 2);

					state.ActionState.SetOverridePersistent();
					state.ActionState.SetOverrideLoss();
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.5244223f, 0.7752154f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(GainSparkAbilityBuilder(2).Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}
}