using System.Collections.Generic;
using Fractural.Tasks;

public class FireKnightPerks
{
	public abstract class FireKnightPerk : PerkModel
	{
	}

	public class ReplaceOneMinusOneWithOnePlusZeroStrengthenAlly : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroStrengthenAlly>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroHealOneRangeOneRolling : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroHealOneRangeOneRolling>()
		];
	}

	public class ReplaceTwoPlusZeroWithTwoPlusZeroIfYouAreOnLadderPlusTwoInstead : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroIfYouAreOnLadderPlusTwoInstead>(),
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroIfYouAreOnLadderPlusTwoInstead>()
		];
	}

	public class ReplacePlusZeroWithOnePlusOneFire : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusOneFire>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneWound : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusOneWound>()
		];
	}

	public class ReplaceOnePlusOneWithOnePlusTwoWound : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusTwoWound>()
		];
	}

	public class AddOnePlusTwoFire : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusTwoFire>()
		];
	}

	public class AddOnePlusOneStrengthenAlly : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusOneStrengthenAlly>()
		];
	}

	public class AddTwoPlusZeroWoundRolling : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroWoundRolling>(),
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroWoundRolling>()
		];
	}

	public class IgnoreScenarioEffectsAddOnePlusZeroFireRolling : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroFireRolling>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class IgnoreItemMinusOneEffectsAddOnePlusZeroFireRolling : FireKnightPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<FireKnightAMDCards.PlusZeroFireRolling>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	public class FearlessLeader : FireKnightPerk
	{
		public override int PerkBoxCount => 2;
		protected override string Title => "Fearless Leader";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"After your first turn each scenario and whenever you open a door during your turn, {Icons.Inline(Icons.GetElement(Element.Fire), richTextParameters)}. For the remainder of the round, all attacks targeting you or adjacent allies gain disadvantage.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			bool disadvantage = false;

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					await AbilityCmd.InfuseElement(null, Element.Fire, character);
					disadvantage = true;
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this);
				});

			ScenarioEvents.DoorOpenedEvent.Subscribe(this,
				parameters => parameters.PotentialOpener == character && GameController.Instance.Map.CurrentTurnTaker == character,
				async _ =>
				{
					await AbilityCmd.InfuseElement(null, Element.Fire, character);
					disadvantage = true;
				});

			ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
				parameters => disadvantage && parameters.AbilityState.Target.AlliedWith(character, true) &&
				              RangeHelper.Distance(parameters.AbilityState.Target.Hex, character.Hex) <= 1,
				async parameters =>
				{
					parameters.AbilityState.SingleTargetSetHasDisadvantage();
					await GDTask.CompletedTask;
				});

			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				_ => true,
				async _ =>
				{
					disadvantage = false;
					await GDTask.CompletedTask;
				});
		}
	}
}