using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ElectrostaticBypass : TempestCardModel<ElectrostaticBypass.CardTop, ElectrostaticBypass.CardBottom>
{
	public override string Name => "Electrostatic Bypass";
	public override int Level => 6;
	public override int Initiative => 15;
	protected override int AtlasIndex => 21;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithFilterTargets((_, figure) => figure is Monster monster && monster.MonsterType is MonsterType.Normal or MonsterType.Elite)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure figure = state.ActionState.GetAbilityState<ConditionAbility.State>(0).Target;
					state.SetCustomValue(this, "Figure", figure);

					await AbilityCmd.AddCharacterToken(state, figure,
						_ => $"Add +2{Icons.Inline(Icons.Attack)} to all attacks targeting this figure");

					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.Target == figure,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await GDTask.CompletedTask;
						});

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters => parameters.Figure == figure,
						async _ =>
						{
							await state.ActionState.RequestDiscardOrLose();
						});
				})
				.WithOnDeactivate(async state =>
				{
					await AbilityCmd.RemoveCharacterToken(state, state.GetCustomValue<Figure>(this, "Figure"));
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithConditionalAbilityCheck(state =>
					LoseSparksConditionalAbilityCheck(state.Performer, 2, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Move)}3")))
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4431351f, 0.66371197f)))
				.WithPierce(1, new PierceSquare(this, new Vector2(0.6657668f, 0.6626039f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();
							parameters.AbilityState.SingleTargetAdjustPierce(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"advantage, +1{Icons.Inline(Icons.Pierce)}")))
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(applyFunction: async parameters =>
					{
						await GainSparks(parameters.AbilityState, parameters.AbilityState.SingleTargetPierce);
					}))
				.Build())
		];

		public override int XP => 1;
	}
}