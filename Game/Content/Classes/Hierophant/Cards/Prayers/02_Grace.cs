using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Grace : HierophantPrayerCardModel<Grace.CardTop, Grace.CardBottom>
{
	public override string Name => "Grace";
	protected override int AtlasIndex => 7 - 2;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.SufferDamageParameters.Figure == state.Performer && !state.Performer.IsDead,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer,
								[HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build()]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.48700017f, 0.31299993f)))
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : HierophantPrayerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}
}