using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ElixirOfLife : BrightsparkCardModel<ElixirOfLife.CardTop, ElixirOfLife.CardBottom>
{
	public override string Name => "Elixir of Life";
	public override int Level => 9;
	public override int Initiative => 38;
	protected override int AtlasIndex => 28;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GiveAbilityCardAbility.Builder()
				.WithGetAbilityCards((state, list) =>
				{
					list.Add(GetAbilityCard(state));
				})
				.WithOnCardGiven(OnCardGiven)
				.WithOnCardDiscarded(OnCardDiscarded)
				.WithOnCardLost(OnCardLost)
				.WithSelectAutomatically(true)
				.Build()
			),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure target = state.ActionState.GetAbilityState<GiveAbilityCardAbility.State>(0).UniqueTargetedFigures[0];

					ScenarioEvents.BeforeFigureKilledEvent.Subscribe(state, this,
						parameters => parameters.Figure == target,
						async parameters =>
						{
							parameters.SetPrevented();
							target.SetHealth((target.MaxHealth + 1) / 2);
							List<AbilityCard> selectedAbilityCards =
								await AbilityCmd.SelectAbilityCards(state.Performer as Character, CardState.Lost, 0, 4,
									hintText: "Select up to four lost cards to recover");

							foreach(AbilityCard selectedAbilityCard in selectedAbilityCards)
							{
								await AbilityCmd.ReturnToHand(selectedAbilityCard);
							}

							await state.AdvanceUseSlot();
						});


					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.BeforeFigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.32444444f, 0.45978832f)))
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override bool Persistent => true;
		public override bool Unrecoverable => true;
		public override bool Loss => true;

		private async GDTask OnCardGiven(AbilityState abilityState, AbilityCard abilityCard)
		{
			Character originalOwner = GetOriginalOwner(abilityState);
			originalOwner.RemoveCard(abilityCard);

			await GDTask.CompletedTask;
		}

		private async GDTask OnCardDiscarded(AbilityCard abilityCard)
		{
			abilityCard.Owner.RemoveCard(abilityCard);

			Character originalOwner = abilityCard.OriginalOwner;
			originalOwner.AddCard(abilityCard);

			await GDTask.CompletedTask;
		}

		private async GDTask OnCardLost(AbilityCard abilityCard)
		{
			abilityCard.Owner.RemoveCard(abilityCard);

			Character originalOwner = abilityCard.OriginalOwner;
			originalOwner.AddCard(abilityCard);

			await GDTask.CompletedTask;
		}
	}

	public class CardBottom : BrightsparkCardSide
	{
		private static readonly int[] MoveValues = [2, 1, 2];
		private static readonly int[] HealValues = [1, 2, 2];

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(3)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Light, effectInfoText: $"Perform {Icons.Inline(Icons.Heal)}3 ability"))
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(parameters.Figure,
							[
								MoveAbility.Builder().WithDistance(MoveValues[state.UseSlotIndex]).Build(),
								HealAbility.Builder().WithHealValue(HealValues[state.UseSlotIndex]).WithTarget(Target.Self).Build()
							]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.26814815f, 0.89629626f)),
					new UseSlot(new Vector2(0.47555554f, 0.89629626f)),
					new UseSlot(new Vector2(0.6844444f, 0.89629626f)),
				])
				.Build()),
		];

		public override bool Persistent => true;
	}
}