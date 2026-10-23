using System.Collections.Generic;
using Fractural.Tasks;

public class RimehearthPerks
{
	public abstract class RimehearthPerk : PerkModel
	{
	}

	public class ReplaceOneMinusOneWithOnePlusZeroChill : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusZeroChill>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroWoundRolling : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusZeroWoundRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneIce : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusOneIce>()
		];
	}

	public class ReplaceTwoPlusZeroWithTwoPlusZeroFireRolling : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusZeroFireRolling>(),
			ModelDB.AMDCard<RimehearthAMDCards.PlusZeroFireRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusZeroHealThreeSelfWoundSelfRolling : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusZeroHealThreeSelfWoundSelfRolling>()
		];
	}

	public class ReplaceThreePlusThreeWithOnePlusOneRollingOnePlusOneWoundOnePlusOneHealOneSelf : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
			ModelDB.AMDCard<PlusOneAMDCard>(),
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusOneRolling>(),
			ModelDB.AMDCard<RimehearthAMDCards.PlusOneWound>(),
			ModelDB.AMDCard<RimehearthAMDCards.PlusOneHealOneSelf>(),
		];
	}

	public class ReplaceOnePlusTwoWithPlusOneThreeChill : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusThreeChill>()
		];
	}

	public class AddOnePlusTwoFireIce : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusTwoFireIce>()
		];
	}

	public class AddOnePlusZeroBrittle : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusZeroBrittle>()
		];
	}

	public class IgnoreItemMinusOneEffectsAddOnePlusZeroFireIceRolling : RimehearthPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RimehearthAMDCards.PlusZeroFireIceRolling>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	public class Icebreaker : RimehearthPerk
	{
		protected override string Title => "Icebreaker";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"At the start of each scenario, you may either gain {Icons.InlineCondition(Conditions.Wound1, richTextParameters)} to {Icons.InlineElement(Element.Fire, richTextParameters)}, or gain {Icons.InlineCondition(Conditions.Chill, richTextParameters)} to {Icons.InlineElement(Element.Ice, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription fireSubscription =
				ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
					subscriptionParameters => true,
					async subscriptionParameters =>
					{
						await AbilityCmd.AddCondition(null, character, Conditions.Wound1);
						await AbilityCmd.InfuseElement(null, Element.Fire, character);
					},
					effectType: EffectType.Selectable,
					effectButtonParameters: new TextEffectButton.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Wound1))}{Icons.Inline(Icons.GetElement(Element.Fire))}"),
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"Gain {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} to {Icons.Inline(Icons.GetElement(Element.Fire))}")
				);

			ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription iceSubscription =
				ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
					subscriptionParameters => true,
					async subscriptionParameters =>
					{
						await AbilityCmd.AddCondition(null, character, Conditions.Chill);
						await AbilityCmd.InfuseElement(null, Element.Ice, character);
					},
					effectType: EffectType.Selectable,
					effectButtonParameters: new TextEffectButton.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Chill))}{Icons.Inline(Icons.GetElement(Element.Ice))}"),
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"Gain {Icons.Inline(Icons.GetCondition(Conditions.Chill))} to {Icons.Inline(Icons.GetElement(Element.Ice))}")
				);

			await AbilityCmd.GenericChoice(character, [fireSubscription, iceSubscription]);
		}
	}
}