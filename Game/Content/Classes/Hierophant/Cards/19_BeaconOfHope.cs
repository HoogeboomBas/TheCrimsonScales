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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.43511614f, 0.20235652f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.66057616f, 0.20235652f)))
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.28136954f, 0.32378387f)))
				.WithCustomGetTargets((state, list) =>
				{
					HealAbility.State healAbilityState = state.ActionState.GetAbilityState<HealAbility.State>(0);
					foreach(Figure targetedFigure in healAbilityState.UniqueTargetedFigures)
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

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Bless, Conditions.Bless])
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
					{
						ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
							canApply: canApplyParameters =>
								state.Performer.AlliedWith(canApplyParameters.Performer) &&
								canApplyParameters.AMDCard.Model is BlessAMDCard,
							apply: async applyParameters =>
							{
								ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
									canApply: canApplyParameters =>
										canApplyParameters.AbilityState == applyParameters.AbilityState,
									apply: async parameters =>
									{
										ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);

										ActionState actionState = new ActionState(parameters.AbilityState.Performer,
											[
												HealAbility.Builder()
													.WithHealValue(6)
													.WithTarget(Target.Self)
													.Build()
											]
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

		public override int XP => 1;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}