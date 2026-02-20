using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SurvivalInstincts : RuinmawCardModel<SurvivalInstincts.CardTop, SurvivalInstincts.CardBottom>
{
	public override string Name => "Survival Instincts";
	public override int Level => 7;
	public override int Initiative => 13;
	protected override int AtlasIndex => 24;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(3)
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer && canApplyParameters.SufferDamageParameters.FromAttack,
						async applyParameters =>
						{
							ScenarioEvents.AbilityEndedEvent.Subscribe(state, this,
								parameters => applyParameters.PotentialAbilityState == parameters.AbilityState,
								async parameters =>
								{
									ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);
									ActionState actionState = new ActionState(state.Performer,
									[
										HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).WithConditions(Ruinmaw.Empower).Build(),
									]);
									await actionState.Perform();
									await state.AdvanceUseSlot();
								}
							);
							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.3825001f, 0.41549924f)),
					new UseSlot(new Vector2(0.5885006f, 0.41549924f))
				])
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Rupture)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
						        .Where(figure => figure.EnemiesWith(state.Performer)))
					{
						await AbilityCmd.SufferDamage(state, figure, 1);
						state.SetPerformed();
					}

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return IsSated(state.Performer);
				})
				.WithOnAbilityEndedPerformed(async state =>
					{
						state.ActionState.SetOverrideRound();

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];
	}
}