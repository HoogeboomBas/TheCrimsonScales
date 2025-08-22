using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Ordination : HierophantPrayerCardModel<Ordination.CardTop, Ordination.CardBottom>
{
	public override string Name => "Ordination";
	protected override int AtlasIndex => 7 - 5;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.398f, 0.306f)), new UseSlot(new Vector2(0.603f, 0.306f))],
				async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [MoveAbility.Builder().WithDistance(2).Build()]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

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
			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.398f, 0.772f)), new UseSlot(new Vector2(0.603f, 0.772f))],
				async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [MoveAbility.Builder().WithDistance(2).Build()]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override bool Persistent => true;
	}
}