using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class AdvancedResearch : BrightsparkCardModel<AdvancedResearch.CardTop, AdvancedResearch.CardBottom>
{
	public override string Name => "Advanced Research";
	public override int Level => 5;
	public override int Initiative => 85;
	protected override int AtlasIndex => 20;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<Figure> figures =
					[
						await AbilityCmd.SelectFigure(state,
							list => list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 3)
								.Where(figure => figure is Character && figure.AlliedWith(state.Performer, true))),
							hintText: () => "Choose an ally or self to target")
					];
					if(state.GetCustomValue<bool>(this, "ExtraTarget"))
					{
						figures.Add(await AbilityCmd.SelectFigure(state,
							list => list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 3)
								.Where(figure => figure is Character && figure.AlliedWith(state.Performer) && !figures.Contains(figure))),
							hintText: () => "Choose another ally to target"));
					}

					int selectionCount = 1 + state.GetCustomValue<int>(this, "ExtraCard");
					bool recoverItem = state.GetCustomValue<bool>(this, "RecoverItem");

					foreach(Figure figure in figures.Where(figure => figure != null))
					{
						List<AbilityCard> selectedAbilityCards =
							await AbilityCmd.SelectAbilityCards(figure as Character, CardState.Discarded, 0, selectionCount,
								hintText: $"Select up to {selectionCount} discarded cards to recover");
						foreach(AbilityCard abilityCard in selectedAbilityCards)
						{
							await AbilityCmd.ReturnToHand(abilityCard);
							state.SetPerformed();
						}

						if(recoverItem)
						{
							ItemModel item = await AbilityCmd.SelectItem(figure as Character, ItemState.Spent, hintText: "Select an item to recover");
							if(item != null)
							{
								await AbilityCmd.RefreshItem(item);
								state.SetPerformed();
							}
						}
					}
				})
				.WithAbilityStartedSubscriptions(
				[
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.SetCustomValue(this, "ExtraTarget", true);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Affect one additional ally within {Icons.Inline(Icons.Range)}3")),
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.SetCustomValue(this, "RecoverItem", true);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"They may also {Icons.Inline(Icons.RecoverCard)} one spent item")),
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(
						[CardElementConsumption.ConsumeWild(), CardElementConsumption.ConsumeWild()],
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.SetCustomValue(this, "ExtraCard", 1);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Two cards from their discard pile instead"))
				])
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<AbilityCard> selectedAbilityCards =
						await AbilityCmd.SelectAbilityCards(state.Performer as Character, CardState.Discarded, 0, 2,
							hintText: "Select up to two discarded cards to recover");

					foreach(AbilityCard selectedAbilityCard in selectedAbilityCards)
					{
						await AbilityCmd.ReturnToHand(selectedAbilityCard);

						state.SetPerformed();
					}
				})
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.LongRestStartedEvent.Subscribe(state, this,
						parameters => parameters.Character == state.Performer,
						async parameters =>
						{
							await AbilityCmd.InfuseWildElement(state);
							AbilityCard card = await AbilityCmd.SelectAbilityCard(parameters.Character, CardState.Discarded,
								hintText: $"Select a card to {Icons.HintText(Icons.LoseCard)} to {Icons.HintText(Icons.RecoverCard)} a lost card");
							if(card != null)
							{
								await card.SetCardState(CardState.Lost);
								AbilityCard recoveredCard = await AbilityCmd.SelectAbilityCard(parameters.Character, CardState.Lost, true,
									hintText: $"{Icons.HintText(Icons.RecoverCard)} a card from your lost pile");
								if(recoveredCard != null)
								{
									await AbilityCmd.ReturnToHand(card);
								}
							}
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.LongRestStartedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild(), CardElementInfusion.InfuseWild()];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}