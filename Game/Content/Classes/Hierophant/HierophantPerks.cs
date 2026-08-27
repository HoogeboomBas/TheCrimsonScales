using System.Collections.Generic;
using Fractural.Tasks;

public class HierophantPerks
{
	public abstract class HierophantPerk : PerkModel
	{
	}

	public class RemoveTwoMinusOne : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
			ModelDB.AMDCard<MinusOneAMDCard>()
		];
	}

	public class ReplaceOneMinusTwoWithOneMinusOneGivePrayerCardAndOnePlusZero : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.MinusOneGivePrayerCard>(),
			ModelDB.AMDCard<HierophantAMDCards.PlusZero>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroCurse : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.PlusZeroCurse>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusZeroLightRolling : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.PlusZeroLightRolling>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusZeroEarthRolling : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.PlusZeroEarthRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneGrantOneAllyShieldOne : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.PlusOneGrantOneAllyShieldOne>()
		];
	}

	public class ReplaceOnePlusOneWithOnePlusThree : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.PlusThree>()
		];
	}

	public class AddOnePlusOneWoundMuddle : HierophantPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.PlusOneWoundMuddle>()
		];
	}

	public class AddTwoPlusZeroHealOneAllyOrSelfRolling : HierophantPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HierophantAMDCards.PlusZeroHealOneAllyOrSelfRolling>(),
			ModelDB.AMDCard<HierophantAMDCards.PlusZeroHealOneAllyOrSelfRolling>()
		];
	}

	public class IgnoreScenarioEffectsRemoveOnePlusZero : HierophantPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class GiftOfTheOak : HierophantPerk
	{
		protected override string Title => "Gift of the Oak";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"At the start of each scenario, perform: {Icons.Inline(Icons.GetCondition(Conditions.Bless), richTextParameters)}, {Icons.Inline(Icons.Range, richTextParameters)}2.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			await new ActionState(character, [ConditionAbility.Builder().WithConditions(Conditions.Bless).WithRange(2).Build()]).Perform();
		}
	}
}