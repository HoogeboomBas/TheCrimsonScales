using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BeaconOfHope : HierophantLevelUpCardModel<BeaconOfHope.CardTop, BeaconOfHope.CardBottom>
{
	public override string Name => "Beacon of Hope";
	public override int Level => 4;
	public override int Initiative => 82;
	protected override int AtlasIndex => 15 - 5;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRangeType(RangeType.Range)
				.WithCustomGetTargets((state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					foreach(Figure targetedFigure in attackAbilityState.UniqueTargetedFigures)
					{
						if(!targetedFigure.IsDead)
						{
							foreach(Figure adjacentFigure in RangeHelper.GetFiguresInRange(targetedFigure.Hex, 1))
							{
								list.Add(adjacentFigure);
							}
						}
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Light];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Bless, Conditions.Bless)
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
					{
						ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
							canApply: canApplyParameters =>
								state.Performer.AlliedWith(canApplyParameters.Performer) &&
								canApplyParameters.AMDCard is BlessAMDCard,
							apply: async applyParameters =>
							{
								ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
									canApply: canApplyParameters =>
										canApplyParameters.AbilityState == applyParameters.AbilityState,
									apply: async parameters =>
									{
										ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);

										ActionState actionState = new ActionState(parameters.AbilityState.Performer, 
											[HealAbility.Builder()
												.WithHealValue(6)
												.WithTarget(Target.Self)
												.Build()]
										);

										await actionState.Perform();

										await state.AdvanceUseSlot();
									}
								);

								await GDTask.CompletedTask;
							}
						);

						await GDTask.CompletedTask;
					}
				)
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);
						ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.9f)))
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}