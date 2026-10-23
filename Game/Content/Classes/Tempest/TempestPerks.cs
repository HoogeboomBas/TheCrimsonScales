using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class TempestPerks
{
	public abstract class TempestPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOneMinusOneAirLight : TempestPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.MinusOneAirLight>()
		];
	}

	public class ReplaceOneMinusOneAirLightWithOnePlusOneAirLight : TempestPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.MinusOneAirLight>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.PlusOneAirLight>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroWound : TempestPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.PlusZeroWound>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroRegenerateRangeOneRolling : TempestPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.PlusZeroRegenerateRangeOneRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusTwoMuddle : TempestPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.PlusTwoMuddle>()
		];
	}

	public class ReplaceTwoPlusZeroWithOnePlusOneImmobilize : TempestPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.PlusOneImmobilize>()
		];
	}

	public class AddOnePlusOneDodgeSelf : TempestPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.PlusOneDodgeSelf>()
		];
	}

	public class AddOnePlusTwoAirLight : TempestPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<TempestAMDCards.PlusTwoAirLight>()
		];
	}

	public class Recharged : TempestPerk
	{
		public override int PerkBoxCount => 2;
		protected override string Title => "Recharged";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, gain {Icons.InlineCondition(Conditions.Dodge, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestEndedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					await AbilityCmd.AddCondition(null, parameters.Character, Conditions.Dodge);
				});
		}
	}

	public class LightningStrike : TempestPerk
	{
		protected override string Title => "Lightning Strike";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you short rest, you may spend 1{Icons.Inline(Tempest.SparkIconPath, richTextParameters)} to cause one enemy within {Icons.Inline(Icons.Range, richTextParameters)}2 to suffer {Icons.Inline(Icons.Damage)}1.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character && TempestCardSide.HasXSparks(character, 1),
				async parameters =>
				{
					Tempest tempest = (Tempest)parameters.Character;
					TempestCardSide.LoseSparks(character);
					Figure figure = await AbilityCmd.SelectFigure(tempest,
						figures => figures.AddRange(RangeHelper.GetFiguresInRange(tempest, 2, false).Where(figure => tempest.EnemiesWith(figure))),
						hintText: () => $"Select an enemy to suffer {Icons.Inline(Icons.Damage)}1");
					if(figure != null)
					{
						await AbilityCmd.SufferDamage(figure, 1, tempest);
					}
				}, EffectType.Selectable,
				effectButtonParameters: new TextEffectButton.Parameters($"1{Icons.HintText(Tempest.SparkIconPath)}"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters(
					$"One enemy within {Icons.Inline(Icons.Range)}2 suffers {Icons.Inline(Icons.Damage)}1"));
		}
	}

	public class EnergizingEvasion : TempestPerk
	{
		protected override string Title => "Energizing Evasion";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you prevent an attack using {Icons.InlineCondition(Conditions.Dodge, richTextParameters)}, gain 1{Icons.Inline(Tempest.SparkIconPath, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.DodgeUsedEvent.Subscribe(this,
				parameters => parameters.DodgeUser == character,
				async parameters =>
				{
					if(parameters.DodgeUser is Tempest tempest)
					{
						tempest.GainSparks();
					}

					await GDTask.CompletedTask;
				});
		}
	}
}