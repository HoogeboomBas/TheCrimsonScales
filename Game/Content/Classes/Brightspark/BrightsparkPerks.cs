using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BrightsparkPerks
{
	public abstract class BrightsparkPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOneMinusTwoRecoverRandomCardFromDiscard : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.MinusTwoRecoverRandomCardFromDiscard>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroConsumeElementForPlusTwo : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroConsumeElementForPlusTwo>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneHealOneAllyRangeTwo : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusOneHealOneAllyRangeTwo>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusOneGrantOneAllyWithinRangeTwoShieldOne : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusOneGrantOneAllyWithinRangeTwoShieldOne>()
		];
	}

	public class ReplaceTwoPlusZeroWithThreePlusZeroConsumeElementToInfuseElementRolling : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroConsumeElementToInfuseElementRolling>(),
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroConsumeElementToInfuseElementRolling>(),
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroConsumeElementToInfuseElementRolling>()
		];
	}

	public class ReplaceOnePlusOneWithOnePlusTwoWildElement : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusTwoWildElement>()
		];
	}

	public class ReplaceTwoPlusOneWithTwoPlusOneStrengthenAllyRangeTwo : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusOneStrengthenAllyRangeTwo>(),
			ModelDB.AMDCard<BrightsparkAMDCards.PlusOneStrengthenAllyRangeTwo>()
		];
	}

	public class AddOnePlusZeroImmobilizeIceRollingOnePlusZeroPushOneOrPullOneAirRolling : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroImmobilizeIceRolling>(),
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroPushOneOrPullOneAirRolling>()
		];
	}

	public class AddOnePlusZeroPierceTwoFireRollingOnePlusZeroHealOneRangeThreeLightRolling : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroPierceTwoFireRolling>(),
			ModelDB.AMDCard<BrightsparkAMDCards.PlusZeroHealOneRangeThreeLightRolling>()
		];
	}

	public class IgnoreScenarioEffectsRemoveOneMinusOne : BrightsparkPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class SparkOfInspiration : BrightsparkPerk, IEventSubscriber
	{
		public override int PerkBoxCount => 2;
		protected override string Title => "Spark of Inspiration";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, after you recover your cards from your discard pile, you may play one card from your hand to perform all persistent abilities on either the top or bottom action of the card.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestEndedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					AbilityCard selectedAbilityCard = await AbilityCmd.SelectAbilityCard(character, CardState.Hand,
						canSelectFunc: card => card.Top.Model.Persistent || card.Bottom.Model.Persistent,
						hintText: "Select a card to perform all persistent abilities on either the top or bottom action");

					if(selectedAbilityCard == null)
					{
						return;
					}

					List<CardPlayCardData> cardDatas =
					[
						new CardPlayCardData
						{
							AbilityCard = selectedAbilityCard,
							CanPlayTop = selectedAbilityCard.Top.Model.Persistent,
							CanPlayBottom = selectedAbilityCard.Bottom.Model.Persistent,
							CanPlayBasicTop = false,
							CanPlayBasicBottom = false,
						}
					];

					EffectCollection cardSideSelectionEffectCollection =
						ScenarioEvents.CardSideSelectionEvent.CreateEffectCollection(
							new ScenarioEvents.CardSideSelection.Parameters(character));

					AbilityCardSectionSelectionPrompt.Answer cardSectionAnswer = await PromptManager.Prompt(
						new AbilityCardSectionSelectionPrompt(cardDatas, cardSideSelectionEffectCollection, () => "Select card side to play"),
						character);

					AbilityCard card = GameController.Instance.ReferenceManager.Get<AbilityCard>(cardSectionAnswer.CardReferenceId);
					AbilityCardSection section = cardSectionAnswer.AbilityCardSection;

					if(!GameController.FastForward)
					{
						Log.Write($"Playing {card.Model.Name} {section}.");
					}

					ScenarioEvents.AbilityStartedEvent.Subscribe(this,
						abilityStartedParameters => (abilityStartedParameters.AbilityState.ActionState.ActionSource == card.Top ||
						                            abilityStartedParameters.AbilityState.ActionState.ActionSource == card.Bottom) &&
						                            abilityStartedParameters.AbilityState is not ActiveAbilityState,
						async abilityStartedParameters =>
						{
							abilityStartedParameters.SetIsBlocked(true);
							await GDTask.CompletedTask;
						});

					switch(section)
					{
						case AbilityCardSection.Top:
							await card.Top.Perform(character);
							break;
						case AbilityCardSection.Bottom:
							await card.Bottom.Perform(character);
							break;
						case AbilityCardSection.BasicTop:
							await card.BasicTop.Perform(character);
							break;
						case AbilityCardSection.BasicBottom:
							await card.BasicBottom.Perform(character);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
				});
		}
	}
}