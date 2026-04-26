using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class LuminaryPerks
{
	public abstract class LuminaryPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOneMinusTwoPerformGlowAbilityWithoutConsumingElement : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.MinusTwoPerformGlowAbilityWithoutConsumingElement>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroFire : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroFire>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroIce : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroIce>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroLight : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroLight>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroDark : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroDark>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusZeroWild : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroWild>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusTwo : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusTwo>()
		];
	}

	public class AddOnePlusZeroPerformPoisonAbility : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroPerformPoisonAbility>(),
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroPerformPoisonAbility>()
		];
	}

	public class AddOnePlusOneHealOneSelfRolling : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusOneHealOneSelfRolling>(),
			ModelDB.AMDCard<LuminaryAMDCards.PlusOneHealOneSelfRolling>()
		];
	}

	public class IgnoreScenarioEffectsAddOnePlusZeroConsumeElementToInfuseElementRolling : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<LuminaryAMDCards.PlusZeroConsumeElementToInfuseElementRolling>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class IgnoreItemEffectsRemoveOnePlusZero : LuminaryPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	public class IgnitedPower : LuminaryPerk, IEventSubscriber
	{
		protected override string Title => "Ignited Power";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you perform an action with a {Icons.Inline(Icons.LoseCard, richTextParameters)} icon, you may immediately play a card from your hand to perform an action that has a {Icons.Inline(LuminaryCardSide.GlowIconPath, richTextParameters)} ability.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(this,
				parameters => parameters.Performer == character && parameters.ResultingState.IsLoss(),
				async _ =>
				{
					AbilityCard selectedAbilityCard = await AbilityCmd.SelectAbilityCard(character, CardState.Hand,
						canSelectFunc: abilityCard => abilityCard.Model.Top.Abilities.Any(ability => ability.Ability is GlowActiveAbility));
					if(selectedAbilityCard == null)
					{
						return;
					}

					AbilityCard card = GameController.Instance.ReferenceManager.Get<AbilityCard>(selectedAbilityCard.ReferenceId);

					if(!GameController.FastForward)
					{
						Log.Write($"Playing {card.Model.Name} {AbilityCardSection.Top}.");
					}

					await card.Top.Perform(character);
				});
		}
	}
}