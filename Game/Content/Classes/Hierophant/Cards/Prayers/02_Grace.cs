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
			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.5f, 0.308f))],
				async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.SufferDamageParameters.Figure == state.Performer
							&& !state.Performer.IsDead
							&& !canApplyParameters.SufferDamageParameters.DamagePrevented,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [new HealAbility(3, target: Target.Self)]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
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