using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Aspiration : HierophantPrayerCardModel<Aspiration.CardTop, Aspiration.CardBottom>
{
	public override string Name => "Aspiration";
	protected override int AtlasIndex => 7 - 0;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Target == state.Performer && canApplyParameters.Condition.IsNegative,
						async applyParameters =>
						{
							if(!applyParameters.Prevented)
							{
								applyParameters.SetPrevented(true);

								ActionState actionState = new ActionState(state.Performer, [HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()]);
								await actionState.Perform();

								await state.AdvanceUseSlot();
							}

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.398f, 0.314f)),
						new UseSlot(new Vector2(0.603f, 0.314f))
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Target == state.Performer && canApplyParameters.Condition.IsNegative,
						async applyParameters =>
						{
							if(!applyParameters.Prevented)
							{
								applyParameters.SetPrevented(true);

								ActionState actionState = new ActionState(state.Performer, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]);
								await actionState.Perform();

								await state.AdvanceUseSlot();
							}

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.764f)))
				.Build())
		];

		protected override bool Persistent => true;
	}
}