using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class SpiritCallerPerks
{
	public abstract class SpiritCallerPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOnePlusZero : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>() // Using default one, so it can be replaced with another AMD later
			//ModelDB.AMDCard<SpiritCallerAMDCards.PlusZero>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroPlusTwoIfSpiritAttacked : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusZeroPlusTwoIfSpiritAttacked>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroPoisonRolling : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusZeroPoisonRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneAir : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusOneAir>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneDark : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusOneDark>()
		];
	}

	public class ReplaceTwoPlusZeroWithTwoPlusZeroPierceThreeRolling : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusZeroPierceThreeRolling>(),
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusZeroPierceThreeRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusZeroAddTargetRolling : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusZeroAddTargetRolling>()
		];
	}

	public class ReplaceTwoPlusOneWithOnePlusOnePierceTwo : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusOnePierceTwo>()
		];
	}

	public class AddOnePlusTwoPushTwo : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusTwoPushTwo>()
		];
	}

	public class IgnoreScenarioEffectsAddOnePlusOneCurse : SpiritCallerPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<SpiritCallerAMDCards.PlusOneCurse>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class SpectralVelocity : SpiritCallerPerk
	{
		protected override string Title => "Spectral Velocity";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you end your turn in a hex occupied by a Spirit, the Spirit gains +2{Icons.Inline(Icons.Move, richTextParameters)} on its next move ability that round.";

		public override int PerkBoxCount => 2;

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(character, this,
				parameters =>
					parameters.Figure == character &&
					Spirit.HasSpirit(parameters.Figure.Hex),
				async parameters =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(character, this);

					Figure spirit = parameters.Figure.Hex.GetFigures(true).FirstOrDefault(figure => Spirit.CountsAsSpirit(figure));

					ScenarioEvents.AbilityStartedEvent.Subscribe(character, this,
						abilityStartedParameters =>
							abilityStartedParameters.AbilityState.Performer == spirit &&
							abilityStartedParameters.AbilityState is MoveAbility.State,
						async abilityStartedParameters =>
						{
							((MoveAbility.State)abilityStartedParameters.AbilityState).AdjustMoveValue(2);
							ScenarioEvents.AbilityStartedEvent.Unsubscribe(character, this);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				}
			);
		}
	}
}