using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ShardrenderPerks
{
	public abstract class ShardrenderPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOnePlusZero : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ShardrenderAMDCards.PlusZero>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusOne : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ShardrenderAMDCards.PlusOne>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroShieldOneRolling : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ShardrenderAMDCards.PlusZeroShieldOneRolling>()
		];
	}

	public class ReplaceTwoPlusZeroWithTwoPlusZeroMoveCharacterTokenOnCrystallizeBackwardOneSlot : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ShardrenderAMDCards.PlusZeroMoveCharacterTokenOnCrystallizeBackwardOneSlot>(),
			ModelDB.AMDCard<ShardrenderAMDCards.PlusZeroMoveCharacterTokenOnCrystallizeBackwardOneSlot>(),
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneIfAttackHasPiercePlusTwoInstead : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ShardrenderAMDCards.PlusOneIfAttackHasPiercePlusTwoInsteadRolling>(),
		];
	}


	public class AddTwoPlusOneAdvanceCrystallizePlusOneAttack : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ShardrenderAMDCards.PlusOneAdvanceCrystallizePlusOneAttack>(),
			ModelDB.AMDCard<ShardrenderAMDCards.PlusOneAdvanceCrystallizePlusOneAttack>()
		];
	}

	public class AddPlusZeroBrittle : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ShardrenderAMDCards.PlusZeroBrittle>()
		];
	}

	public class IgnoreItemMinusOneEffectsRemovePlusZero : ShardrenderPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	public class InvigoratingMeditation : ShardrenderPerk
	{
		protected override string Title => "Invigorating Meditation";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, gain {Icons.InlineCondition(Conditions.Regenerate, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async _ =>
				{
					await AbilityCmd.AddCondition(null, character, Conditions.Regenerate, character);
				});

			await GDTask.CompletedTask;
		}
	}

	public class Solidify : ShardrenderPerk
	{
		protected override string Title => "Solidify";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Once each scenario, when you would suffer {Icons.Inline(Icons.Damage, richTextParameters)} from an attack, gain {Icons.Inline(Icons.Shield, richTextParameters)}3 for that attack.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.SufferDamageEvent.Subscribe(this,
				parameters => parameters.FromAttack && parameters.Figure == character && parameters.WouldSufferDamage,
				async parameters =>
				{
					parameters.AdjustShield(2);

					ScenarioEvents.SufferDamageEvent.Unsubscribe(this);

					await GDTask.CompletedTask;
				}, EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.Shield),
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"Use Solidify to gain {Icons.Inline(Icons.Shield)}3"));

			await GDTask.CompletedTask;
		}
	}

	public class TakeShape : ShardrenderPerk
	{
		public override int PerkBoxCount => 2;
		protected override string Title => "Take Shape";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"At the start of each scenario, you may play a level 1 card from your hand to perform a {Icons.Inline(ShardrenderCardSide.CrystallizeIconPath, richTextParameters)} action of the card and gain {Icons.Inline(Icons.InlineCondition(Conditions.Ward, richTextParameters))}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(character, CardState.Hand,
				canSelectFunc: abilityCard => abilityCard.Model.Level == 1 &&
				                              abilityCard.Top.Model.Abilities.Any(abilityCardAbility =>
					                              abilityCardAbility.Ability is CrystallizeAbility) ||
				                              abilityCard.Bottom.Model.Abilities.Any(abilityCardAbility =>
					                              abilityCardAbility.Ability is CrystallizeAbility),
				hintText: $"Select a level 1 card with a {Icons.HintText(ShardrenderCardSide.CrystallizeIconPath)} action to play");
			if(abilityCard != null)
			{
				if(abilityCard.Top.Model.Abilities.Any(abilityCardAbility => abilityCardAbility.Ability is CrystallizeAbility))
				{
					await abilityCard.Top.Perform(character);
				}
				else
				{
					await abilityCard.Bottom.Perform(character);
				}
			}

			await AbilityCmd.AddCondition(null, character, Conditions.Ward, character);
		}
	}
}