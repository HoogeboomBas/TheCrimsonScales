using System.Collections.Generic;
using Fractural.Tasks;

public class ChieftainPerks
{
	public abstract class ChieftainPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOneMinusTwoBlessSelf : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.MinusTwoBlessSelf>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroPoison : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroPoison>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroHealOneChieftain : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroHealOneChieftain>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroHealTargetAllYourSummons : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroHealTargetAllYourSummons>()
		];
	}

	public class ReplaceTwoPlusZeroWithPlusZeroPushOneImmobilize : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroPushOneImmobilize>()
		];
	}

	public class ReplacePlusZeroWithOnePlusZeroAddPlusOneForEachOfYourSummons : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroAddPlusOneForEachOfYourSummons>()
		];
	}

	public class ReplaceOnePlusZeroWithTwoPlusZeroPierceTwoUnaffectedByRetaliateRolling : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroPierceTwoUnaffectedByRetaliateRolling>(),
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroPierceTwoUnaffectedByRetaliateRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneIfDrawnBySummonRolling : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusOneIfDrawnBySummonRolling>(),
			ModelDB.AMDCard<ChieftainAMDCards.PlusOneIfDrawnBySummonRolling>()
		];
	}

	public class AddTwoPlusOneEarth : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusOneEarth>(),
			ModelDB.AMDCard<ChieftainAMDCards.PlusOneEarth>()
		];
	}

	public class IgnoreScenarioEffectsAddPlusZeroPierceOneWound : ChieftainPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ChieftainAMDCards.PlusZeroPierceOneWound>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class PracticedControl : ChieftainPerk
	{
		protected override string Title => "Practiced Control";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"While you are mounted on a summon, all of your summons within {Icons.Inline(Icons.Range, richTextParameters)}1 of you are unaffected by {Icons.Inline(Icons.Retaliate, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.RetaliateEvent.Subscribe(this,
				parameters => Chieftain.GetIsMounted(character) && parameters.Performer is Summon summon && summon.CharacterOwner == character &&
				              RangeHelper.Distance(summon.Hex, character.Hex) <= 1,
				async parameters =>
				{
					parameters.SetRetaliateBlocked();
					await GDTask.CompletedTask;
				});
		}
	}

	public class Stampede : ChieftainPerk
	{
		protected override string Title => "Stampede";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Once each scenario, during your turn, all of your summons perform {Icons.Inline(Icons.Move, richTextParameters)}+0. You do not control the movement.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			AbilityCmd.SubscribeDuringCharacterTurn(this, EffectType.Selectable, duringTurnCharacter => duringTurnCharacter == character,
				async duringTurnCharacter =>
				{
					foreach(Summon summon in duringTurnCharacter.Summons)
					{
						ActionState actionState = new ActionState(summon, [AbilityCmd.SummonMovePlusX(+0).Build()]);
						await actionState.Perform();
					}

					AbilityCmd.UnsubscribeDuringCharacterTurn(this);
				}, new IconEffectButton.Parameters(Icons.Move),
				new TextEffectInfoView.Parameters($"All of your summon perform {Icons.Inline(Icons.Move)}+0"));
		}
	}
}