using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Penitence : HierophantPrayerCardModel<Penitence.CardTop, Penitence.CardBottom>
{
	public override string Name => "Penitence";
	protected override int AtlasIndex => 7 - 6;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.AbilityState.Performer == state.Performer &&
							canApplyParameters.Type == AMDCardType.Value &&
							canApplyParameters.Value < 0,
						async applyParameters =>
						{
							applyParameters.SetValue(0);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.38450018f, 0.3065f)),
						new UseSlot(new Vector2(0.5899994f, 0.3065f))
					]
				)
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : HierophantPrayerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters =>
							state.Performer.EnemiesWith(canApplyParameters.AbilityState.Performer) &&
							canApplyParameters.Type == AMDCardType.Value &&
							canApplyParameters.Value > 0,
						async applyParameters =>
						{
							applyParameters.SetValue(0);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.38450018f, 0.7780105f)),
						new UseSlot(new Vector2(0.5899994f, 0.7780105f))
					]
				)
				.Build())
		];

		public override bool Persistent => true;
	}
}