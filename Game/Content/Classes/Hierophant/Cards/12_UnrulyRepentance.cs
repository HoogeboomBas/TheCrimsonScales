using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class UnrulyRepentance : HierophantCardModel<UnrulyRepentance.CardTop, UnrulyRepentance.CardBottom>
{
	public override string Name => "Unruly Repentance";
	public override int Level => 1;
	public override int Initiative => 25;
	protected override int AtlasIndex => 13 - 12;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Curse, Conditions.Curse)
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApply: canApplyParameters =>
							state.Performer.EnemiesWith(canApplyParameters.Performer) &&
							canApplyParameters.AMDCard is CurseAMDCard,
						apply: async applyParameters =>
						{
							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
								canApply: canApplyParameters =>
									canApplyParameters.AbilityState == applyParameters.AbilityState,
								apply: async parameters =>
								{
									await AbilityCmd.SufferDamage(null, parameters.Performer, 10);
									await state.AdvanceUseSlot();
								}
							);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this); 

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.4f)))
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherTargetedAbility.Builder()
				.WithOnAfterConditionsApplied(async (state, target) =>
				{
					int conditionCount = 0;

					for(int i = target.Conditions.Count - 1; i >= 0; i--)
					{
						ConditionModel condition = target.Conditions[i];
						if(condition.IsNegative)
						{
							if(await AbilityCmd.RemoveCondition(target, condition))
							{
								conditionCount++;
							}
						}
					}

					state.SetCustomValue(this, "ConditionCount", conditionCount);
				})
				.WithRange(3)
				.WithTarget(Target.Allies)
				.Build()
			),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(
					new DynamicInt<HealAbility.State>(state =>
						state.ActionState.GetAbilityState<OtherTargetedAbility.State>(0).GetCustomValue<int>(this, "ConditionCount"))
				)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithCustomGetTargets((state, list) =>
				{
					list.AddRange(state.ActionState.GetAbilityState<OtherTargetedAbility.State>(0).UniqueTargetedFigures);
				})
				.Build())
		];
	}
}