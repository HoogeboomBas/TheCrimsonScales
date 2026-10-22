using System.Collections.Generic;
using Fractural.Tasks;

public class ThornreaperPerks
{
	public abstract class ThornreaperPerk : PerkModel
	{
	}

	public class ReplaceOneMinusOneWithOnePlusOneIfLightIsStrongOrWaning : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusOneIfLightIsStrongOrWaningRolling>(),
		];
	}

	public class ReplaceOneMinusTwoWithOnePlusZero : ThornreaperPerk
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

	public class ReplaceOnePlusZeroWithOnePlusTwo : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusTwo>()
		];
	}

	public class AddThreePlusOneIfLightIsStrongOrWaning : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusOneIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<ThornreaperAMDCards.PlusOneIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<ThornreaperAMDCards.PlusOneIfLightIsStrongOrWaningRolling>()
		];
	}

	public class AddTwoInfuseLightRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZeroLightRolling>(),
			ModelDB.AMDCard<ThornreaperAMDCards.PlusZeroLightRolling>()
		];
	}

	public class AddThreeInfuseEarthIfLightIsStrongOrWaningRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.InfuseEarthIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<ThornreaperAMDCards.InfuseEarthIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<ThornreaperAMDCards.InfuseEarthIfLightIsStrongOrWaningRolling>(),
		];
	}

	public class AddOneCreateHazardousTerrain : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.CreateHazardousTerrain>()
		];
	}

	public class AddOneRetaliateThreeOnHazardousTerrainRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PersistentRetaliateThreeOnNextAdjacentAttackInHazardousTerrain>()
		];
	}

	public class AddOneShieldThreeOnHazardousTerrainRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PersistentShieldThreeOnNextAttackDamageInHazardousTerrain>(),
		];
	}

	public class IgnoreItemEffectsAndAddOnePlusOneIfLightIsStrongOrWaningRolling : ThornreaperPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ThornreaperAMDCards.PlusOneIfLightIsStrongOrWaningRolling>()
		];

		public override bool IgnoreItemMinusOneEffects => true;

	}

	public class BrambleBulwark : ThornreaperPerk, IEventSubscriber
	{
		protected override string Title => "Bramble Bulwark";
		public override int PerkBoxCount => 2;

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Gain {Icons.Inline(Icons.Shield, richTextParameters)}1 while you occupy hazardous terrain";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			bool shielded = false;

			ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async parameters =>
				{
					bool hazardous = parameters.Hex.HasHexObjectOfType<HazardousTerrain>();

					if (hazardous && !shielded)
					{
						await AbilityCmd.AddShield(character, this, 1);
						shielded = true;
					}
					else if (!hazardous && shielded)
					{
						AbilityCmd.RemoveShield(character, this);
						shielded = false;
					}
				});
			
			if (character.Hex.HasHexObjectOfType<HazardousTerrain>())
			{
				await AbilityCmd.AddShield(character, this, 1);
				shielded = true;
			}
		}
	}

	public class RiseAndShine : ThornreaperPerk, IEventSubscriber
	{
		protected override string Title => "Rise and Shine";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, {Icons.InlineElement(Element.Light, richTextParameters)}";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestEndedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					await AbilityCmd.InfuseElement(null, Element.Light, character);
				});
		}

	}
}