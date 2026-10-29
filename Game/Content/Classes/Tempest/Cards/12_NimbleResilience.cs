using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class NimbleResilience : TempestCardModel<NimbleResilience.CardTop, NimbleResilience.CardBottom>
{
	public override string Name => "Nimble Resilience";
	public override int Level => 1;
	public override int Initiative => 66;
	protected override int AtlasIndex => 12;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealDiamondPlus(this, new Vector2(0.4408069f, 0.16842106f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.65034544f, 0.16776705f)))
				.WithDuringHealSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringHeal.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Regenerate);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Regenerate))}")),

					LoseSparksSubscription<ScenarioEvents.DuringHeal.Parameters>(2, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustHealValue(3);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+3{Icons.Inline(Icons.Heal)}")),

					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringHeal.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Bless);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Bless))}")),
				])
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithConditionalAbilityCheck(state => LoseSparksConditionalAbilityCheck(state.Performer, 2,
					new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Dodge))}, self")))
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.Performer.HasCondition(Conditions.Dodge),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							ScenarioEvents.RetaliateEvent.Subscribe(state, this,
								canApplyParameters => canApplyParameters.AbilityState.SingleTargetState == parameters.AbilityState.SingleTargetState,
								async applyParameters =>
								{
									applyParameters.SetRetaliateBlocked();
									ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
									await GDTask.CompletedTask;
								});

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.50056416f, 0.89407504f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}
}