using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Grace : HierophantPrayerCardModel<Grace.CardTop, Grace.CardBottom>
{
	public override string Name => "Grace";
	protected override int AtlasIndex => 7 - 2;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.SufferDamageParameters.Figure == state.Performer && !state.Performer.IsDead,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [new HealAbility(3, target: Target.Self)]);
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
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.308f)))
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new HealAbility(3, target: Target.Self))
		];

		protected override IEnumerable<Element> Elements => [Element.Light];
	}
}