using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class BellyOfTheBeast : RuinmawCardModel<BellyOfTheBeast.CardTop, BellyOfTheBeast.CardBottom>
{
	public override string Name => "Belly of the Beast";
	public override int Level => 9;
	public override int Initiative => 42;
	protected override int AtlasIndex => 29;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await SateRuinmaw(state.Performer);
					state.SetPerformed();

					int topsPlayed = 0;
					int bottomsPlayed = 0;
					for(int i = 0; i < 4; i++)
					{
						if(state.Performer.IsDead || ((Character)state.Performer).Cards.Count(card => card.CardState == CardState.Lost) == 0)
						{
							break;
						}

						AbilityCard selectedAbilityCard =
							await AbilityCmd.SelectAbilityCard(state.Performer as Character, CardState.Lost, true,
								hintText: $"Select a lost card to play");

						List<CardPlayCardData> cardDatas =
						[
							new CardPlayCardData()
							{
								AbilityCard = selectedAbilityCard,
								CanPlayTop = topsPlayed < 2 && !selectedAbilityCard.Top.Model.Loss,
								CanPlayBottom = bottomsPlayed < 2 && !selectedAbilityCard.Bottom.Model.Loss,
								CanPlayBasicTop = topsPlayed < 2,
								CanPlayBasicBottom = bottomsPlayed < 2,
							}
						];

						EffectCollection cardSideSelectionEffectCollection =
							ScenarioEvents.CardSideSelectionEvent.CreateEffectCollection(
								new ScenarioEvents.CardSideSelection.Parameters(state.Performer as Character));

						AbilityCardSectionSelectionPrompt.Answer cardSectionAnswer = await PromptManager.Prompt(
							new AbilityCardSectionSelectionPrompt(cardDatas, cardSideSelectionEffectCollection, () => "Select card side to play"),
							state.Performer);

						AbilityCard card = GameController.Instance.ReferenceManager.Get<AbilityCard>(cardSectionAnswer.CardReferenceId);
						AbilityCardSection section = cardSectionAnswer.AbilityCardSection;

						if(!GameController.FastForward)
						{
							Log.Write($"Playing {card.Model.Name} {section}.");
						}

						switch(section)
						{
							case AbilityCardSection.Top:
								topsPlayed++;
								await card.Top.Perform(state.Performer);
								break;
							case AbilityCardSection.Bottom:
								bottomsPlayed++;
								await card.Bottom.Perform(state.Performer);
								break;
							case AbilityCardSection.BasicTop:
								await card.BasicTop.Perform(state.Performer);
								topsPlayed++;
								break;
							case AbilityCardSection.BasicBottom:
								await card.BasicBottom.Perform(state.Performer);
								bottomsPlayed++;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						foreach(CardPlayCardData cardData in cardDatas)
						{
							if(cardData.AbilityCard == card)
							{
								cardData.CanPlayTop = false;
								cardData.CanPlayBottom = false;
							}
						}

						if(topsPlayed == 2)
						{
							foreach(CardPlayCardData cardData in cardDatas)
							{
								cardData.CanPlayBottom = false;
							}
						}

						if(bottomsPlayed == 2)
						{
							foreach(CardPlayCardData cardData in cardDatas)
							{
								cardData.CanPlayTop = false;
							}
						}
					}
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.KillOrExhaust(state, state.Performer);
					state.SetPerformed();
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Unrecoverable => true;
		public override bool Loss => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure.EnemiesWith(state.Performer) &&
						                      canApplyParameters.PotentialAbilityState?.Performer == state.Performer,
						async applyParameters =>
						{
							ScenarioEvents.AbilityEndedEvent.Subscribe(state, this, parameters => true,
								async parameters =>
								{
									ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);
									ActionState actionState = new(state.Performer,
									[
										ConditionAbility.Builder()
											.WithConditions(
											[
												Ruinmaw.Empower,
												Ruinmaw.Empower,
												Ruinmaw.Empower,
												Ruinmaw.Empower
											])
											.WithTarget(Target.Self).Build(),
									]);
									await actionState.Perform();
								}
							);
							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.47550005f, 0.8609986f), SateRuinmaw),
				])
				.Build()),
		];

		public override bool Persistent => true;
	}
}