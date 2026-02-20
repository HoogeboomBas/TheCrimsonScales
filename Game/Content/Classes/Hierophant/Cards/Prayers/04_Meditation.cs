using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Meditation : HierophantPrayerCardModel<Meditation.CardTop, Meditation.CardBottom>
{
	public override string Name => "Meditation";
	protected override int AtlasIndex => 7 - 4;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RoundStartedBeforeInitiativesSortedEvent.Subscribe(state, this,
						canApplyParameters => state.Performer is Character character && character.LongResting,
						async applyParameters =>
						{
							await AbilityCmd.AddCondition(state, state.Performer, Conditions.Invisible);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.RoundStartedBeforeInitiativesSortedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.48850015f, 0.2990001f)))
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
					ScenarioEvents.ShortRestStartedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Character == state.Performer,
						async applyParameters =>
						{
							applyParameters.SetCanSelectCardToUse();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.ShortRestStartedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.48850015f, 0.7785105f)))
				.Build())
		];

		public override bool Persistent => true;
	}
}