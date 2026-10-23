using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class IncarnatePerks
{
	public abstract class IncarnatePerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOnePlusZeroRitualistConquerorReaverRolling : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusZeroRitualistConquerorReaverRolling>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroPierceTwoFireRolling : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusZeroPierceTwoFireRolling>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroPushOneAirRolling : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusZeroPushOneAirRolling>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroShieldOneEarthRolling : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusZeroShieldOneEarthRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneRitualistEnfeebleConquerorEmpowerSelf : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusOneRitualistEnfeebleConquerorEmpowerSelf>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneRitualistEnfeebleReaverRupture : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusOneRitualistEnfeebleReaverRupture>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneConquerorEmpowerSelfReaverRupture : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusOneConquerorEmpowerSelfReaverRupture>()
		];
	}

	public class AddOnePlusZeroRecoverOneOrTwoHandItemRolling : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<IncarnateAMDCards.PlusZeroRecoverOneOrTwoHandItemRolling>()
		];
	}

	public class IgnoreItemMinusOneEffectsRemoveOneMinusOne : IncarnatePerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	//TODO: Rename all
	public class NonAMD1 : IncarnatePerk
	{
		protected override string Title => "Non-AMD1";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, perform {Icons.Inline(Incarnate.ThreeSpiritIconPath, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestEndedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async _ =>
				{
					await IncarnateCardSide.ChooseSpirit(character,
						[IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver]);
				});
		}
	}

	public class NonAMD2 : IncarnatePerk
	{
		protected override string Title => "Non-AMD2";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you short rest, {Icons.Inline(Icons.RecoverCard, richTextParameters)} one spent {Icons.Inline(Icons.GetItem(ItemType.OneHand), richTextParameters)} item.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async _ =>
				{
					ItemModel item = await AbilityCmd.SelectItem(character, ItemState.Spent, ItemType.OneHand,
						$"Select an item to {Icons.HintText(Icons.RecoverCard)}");

					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
					}
				});
		}
	}

	public class NonAMD3 : IncarnatePerk
	{
		protected override string Title => "Non-AMD3";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"You may bring one additional {Icons.Inline(Icons.GetItem(ItemType.OneHand), richTextParameters)} into each scenario.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			List<ItemModel> itemModels = [];
			itemModels.AddRange(character.SavedCharacter.ItemIds.Select(itemId => ModelDB.GetById<ItemModel>(itemId).ToMutable())
				.Where(item => item.ItemType == ItemType.OneHand));

			ItemModel itemModel = await AbilityCmd.SelectItem(character, itemModels,
				hintText: $"Select an additional {Icons.HintText(Icons.GetItem(ItemType.OneHand))} item to bring.");

			if(itemModel != null)
			{
				character.EquipItem(itemModel);
			}
		}
	}
}