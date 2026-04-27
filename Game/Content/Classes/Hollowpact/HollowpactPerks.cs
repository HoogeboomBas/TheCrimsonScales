using System.Collections.Generic;

public class HollowpactPerks
{
	public abstract class HollowpactPerk : PerkModel
	{
	}

	public class ReplaceOneMinusOneWithOnePlusZeroHealTwoSelf : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.PlusZeroHealTwoSelf>(),
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusZeroVoidsight : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.PlusZeroVoidsight>()
		];
	}

	public class AddOneMinusTwoEarthAndTwoPlusTwoDark : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.MinusTwoEarth>(),
			ModelDB.AMDCard<HollowpactAMDCards.PlusTwoDark>(),
			ModelDB.AMDCard<HollowpactAMDCards.PlusTwoDark>(),
		];
	}

	public class ReplaceOneMinusOneWithOneMinusTwoStunAndOnePlusZeroVoidsight : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.MinusTwoStun>(),
			ModelDB.AMDCard<HollowpactAMDCards.PlusZeroVoidsight>(),
		];
	}

	public class ReplaceOneMinusTwoWithOnePlusZeroDisarmAndOneMinusOneWildElement : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.PlusZeroDisarm>(),
			ModelDB.AMDCard<HollowpactAMDCards.MinusOneWildElement>(),
		];
	}

	public class ReplaceOneMinusOneWithOnePlusOneVoidEnergyRollingAndOneMinusOneCurseRolling : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.PlusOneVoidEnergyRolling>(),
			ModelDB.AMDCard<HollowpactAMDCards.MinusOneCurseRolling>(),
		];
	}

	public class ReplaceTwoPlusOneWithOnePlusThreeRegenerateSelf : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
			ModelDB.AMDCard<PlusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.PlusThreeRegenerateSelf>(),
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneVoidPitRangeTwo : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.PlusOneVoidPitRangeTwo>(),
		];
	}

	public class IgnoreScenarioEffectsAddOnePlusZeroWardSelf : HollowpactPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<HollowpactAMDCards.PlusZeroWardSelf>(),
		];

		public override bool IgnoreScenarioEffects => true;
	}
}