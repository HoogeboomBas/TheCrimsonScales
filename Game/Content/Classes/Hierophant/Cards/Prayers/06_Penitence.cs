using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Penitence : HierophantPrayerCardModel<Penitence.CardTop, Penitence.CardBottom>
{
	public override string Name => "Penitence";
	protected override int AtlasIndex => 7 - 6;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.398f, 0.306f)), new UseSlot(new Vector2(0.603f, 0.306f))],
				async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.AbilityState.Performer == state.Performer && 
							canApplyParameters.Value < 0 && 
							!canApplyParameters.AMDCard.IsNull,
						async applyParameters =>
						{
							applyParameters.SetCard(new BasicAMDCard(applyParameters.AMDCard, 0));

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

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
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters =>
							state.Performer.EnemiesWith(canApplyParameters.AbilityState.Performer) &&
							canApplyParameters.Value > 0 &&
							!canApplyParameters.AMDCard.IsCrit,
						async applyParameters =>
						{
							applyParameters.SetCard(new BasicAMDCard(applyParameters.AMDCard, 0));

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override bool Persistent => true;
	}
}