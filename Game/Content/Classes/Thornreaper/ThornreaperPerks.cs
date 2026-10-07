using System.Collections.Generic;
using Fractural.Tasks;

public class ThornreaperPerks
{
	public abstract class ThornreaperPerk : PerkModel
	{
	}

	public class RemoveTwoMinusOne : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
			ModelDB.AMDCard<MinusOneAMDCard>()
		];
	}

	public class ReplaceOneMinusTwoWithOneMinusOneGivePrayerCardAndOnePlusZero : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZero>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroCurse : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZeroCurse>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusZeroLightRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZeroLightRolling>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusZeroEarthRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZeroEarthRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneGrantOneAllyShieldOne : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusOneGrantOneAllyShieldOne>()
		];
	}

	public class ReplaceOnePlusOneWithOnePlusThree : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusThree>()
		];
	}

	public class AddOnePlusOneWoundMuddle : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusOneWoundMuddle>()
		];
	}

	public class AddTwoPlusZeroHealOneAllyOrSelfRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZeroHealOneAllyOrSelfRolling>(),
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZeroHealOneAllyOrSelfRolling>()
		];
	}

	public class IgnoreScenarioEffectsRemoveOnePlusZero : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class GiftOfTheOak : ThornreaperPerk, IEventSubscriber
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