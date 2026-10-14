using System.Collections.Generic;
using Fractural.Tasks;

public class ChainguardPerks
{
	public abstract class ChainguardPerk : PerkModel
	{
	}

	public class ReplaceOneMinusWithOnePlusOneShackle : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusOneShackle>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroIfTargetHasShacklePlusTwo : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroIfTargetHasShacklePlusTwo>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusZeroShieldOneRollingOnePlusZeroRetaliateOneRolling : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroShieldOneRolling>(),
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroRetaliateOneRolling>()
		];
	}

	public class ReplaceTwoPlusZeroWithThreePlusZeroSwingThreeRolling : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroSwingThreeRolling>(),
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroSwingThreeRolling>(),
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroSwingThreeRolling>()
		];
	}

	public class ReplaceOnePlusOneWithOnePlusTwoWound : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusTwoWound>()
		];
	}

	public class AddOnePlusOneIfTargetHasShackleDisarm : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusOneIfTargetHasShackleDisarm>()
		];
	}

	public class AddTwoPlusZeroHealOneSelfRolling : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroHealOneSelfRolling>(),
			ModelDB.AMDCard<ChainguardAMDCards.PlusZeroHealOneSelfRolling>()
		];
	}

	public class AddOnePlusTwoShackle : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusTwoShackle>()
		];
	}

	public class IgnoreItemMinusOneEffectsAddPlusOneCreateDamageTwoTrap : ChainguardPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChainguardAMDCards.PlusOneCreateDamageTwoTrap>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	public class UnyieldingJailer : ChainguardPerk
	{
		public override int PerkBoxCount => 3;
		protected override string Title => "Unyielding Jailer";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"You may have an additional enemy with {Icons.Inline(Icons.GetCondition(Chainguard.Shackle))} at any time.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioCheckEvents.MaxShackleCountCheckEvent.Subscribe(this,
				parameters => parameters.Shackler == character,
				parameters =>
				{
					parameters.AdjustMaxShackleCount(1);
				}
			);
		}
	}
}