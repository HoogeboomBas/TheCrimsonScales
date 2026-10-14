using System.Collections.Generic;
using Fractural.Tasks;

public class MirefootPerks
{
	public abstract class MirefootPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOnePlusZero : MirefootPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusZero>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusOne : MirefootPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusOne>()
		];
	}

	public class ReplaceTwoPlusZeroWithTwoPlusZeroPlusXWhereXIsTargetPoisonValue : MirefootPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroPlusXWhereXIsTargetPoisonValue>(),
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroPlusXWhereXIsTargetPoisonValue>()
		];
	}

	public class ReplaceOnePlusZeroWithTwoPlusZeroCreateDifficultTerrainRolling : MirefootPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroCreateDifficultTerrainRolling>(),
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroCreateDifficultTerrainRolling>()
		];
	}

	public class ReplaceTwoPlusOneWithTwoPlusTwo : MirefootPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>(),
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusTwo>(),
			ModelDB.AMDCard<MirefootAMDCards.PlusTwo>()
		];
	}

	public class ReplaceOnePlusOneWithOnePlusZeroWoundTwo : MirefootPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroWoundTwo>()
		];
	}

	public class AddTwoPlusZeroIfOccupyingDifficultTerrainGainInvisibleRolling : MirefootPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroIfOccupyingDifficultTerrainGainInvisibleRolling>(),
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroIfOccupyingDifficultTerrainGainInvisibleRolling>()
		];
	}

	public class AddFourPlusZeroIfOccupyingDifficultTerrainPlusOneInsteadRolling : MirefootPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroIfOccupyingDifficultTerrainPlusOneInsteadRolling>(),
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroIfOccupyingDifficultTerrainPlusOneInsteadRolling>(),
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroIfOccupyingDifficultTerrainPlusOneInsteadRolling>(),
			ModelDB.AMDCard<MirefootAMDCards.PlusZeroIfOccupyingDifficultTerrainPlusOneInsteadRolling>()
		];
	}

	public class IgnoreScenarioEffectsRemoveOneMinusOne : MirefootPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override bool IgnoreScenarioEffects => true;
	}

	public class SilentStepOfTheBogWraith : MirefootPerk
	{
		protected override string Title => "Silent Step of the Bog Wraith";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Ignore negative conditions and attack modifiers and damage from events and remove one {Icons.Inline(Icons.GetAMDValue("-1"), richTextParameters)} card.";

		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.InflictConditionEventRewardEvent.Subscribe(this,
				parameters => parameters.ConditionModel.IsNegative && parameters.Character == character,
				async parameters =>
				{
					parameters.SetPrevented(true);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.SufferDamageEventRewardEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					parameters.SetPrevented(true);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.AddMinusOnesEventRewardEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					parameters.SetPrevented(true);
					await GDTask.CompletedTask;
				});
		}
	}

	public class HiddenBlade : MirefootPerk
	{
		protected override string Title => "Hidden Blade";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Gain a 'Poison Dagger' item. At the start of each scenario, you may select an additional Dagger {Icons.Inline(Icons.GetItem(ItemType.OneHand), richTextParameters)} item to equip.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			List<ItemModel> itemModels = [];

			foreach(string itemId in character.SavedCharacter.ItemIds)
			{
				ItemModel item = ModelDB.GetById<ItemModel>(itemId).ToMutable();
				if(item.Name.Contains("Dagger"))
				{
					itemModels.Add(item);
				}
			}

			ItemModel itemModel = await AbilityCmd.SelectItem(character, itemModels, hintText: "Select an additional Dagger item to bring.");

			if(itemModel != null)
			{
				character.EquipItem(itemModel);
			}
		}

		public override void OnPerkAcquired(SavedCharacter savedCharacter)
		{
			base.OnPerkAcquired(savedCharacter);
			savedCharacter.AddItem(ModelDB.Item<PoisonDagger>());
		}
	}
}