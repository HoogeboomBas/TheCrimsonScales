using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class UnrulyRepentance : HierophantCardModel<UnrulyRepentance.CardTop, UnrulyRepentance.CardBottom>
{
	public override string Name => "Unruly Repentance";
	public override int Level => 1;
	public override int Initiative => 25;
	protected override int AtlasIndex => 29 - 13;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new ConditionAbility([Conditions.Curse, Conditions.Curse], range: 3)),

			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.5f, 0.4f))],
				async state =>
				{
					ScenarioEvents.AMDTerminalDrawnEvent.Subscribe(state, this,
						canApplyParameters =>
							state.Performer.EnemiesWith(canApplyParameters.Performer) &&
							canApplyParameters.AMDCard is CurseAMDCard,
						async applyParameters =>
						{
							await AbilityCmd.SufferDamage(null, applyParameters.Performer, 10);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.AMDTerminalDrawnEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new OtherTargetedAbility(
				async (state, target) =>
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
				}, range: 3, target: Target.Allies
			)),

			new AbilityCardAbility(new HealAbility(
				new DynamicInt<HealAbility.State>(state => state.ActionState.GetAbilityState<OtherTargetedAbility.State>(0).GetCustomValue<int>(this, "ConditionCount")),
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0),
				customGetTargets: (state, list) =>
				{
					list.AddRange(state.ActionState.GetAbilityState<OtherTargetedAbility.State>(0).UniqueTargetedFigures);
				}
			))
		];
	}
}