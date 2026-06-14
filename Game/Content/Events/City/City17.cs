using System;
using System.Collections.Generic;

public class City17 : CityEventModel<City17.ChoiceA, City17.ChoiceB>
{
	public override int Number => 17;

	public override string Text =>
		"""
		You're perusing the alchemy shops in the Mixed District when you happen upon a storefront window and see a Quatryl you're familiar with named Shiela. She smiles cheerfully as you enter the store and she places several potions on the table.

		"Care to buy one of our newest potions?" she glees. Before you can answer, she pulls out another set of potions from the wall behind her. "Otherwise we have a new program where you can exchange old potions out for new."
		""";

	public class ChoiceBDowntimeShopExchangeReward : DowntimeShopSellPriceReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"One player may return any Minor potion from their possession to the shop and receive a Major potion of the same type for free.";

		private bool _minorPotionReturned = false;

		private static bool IsMinorPotion(ItemModel itemModel) =>
			itemModel == ModelDB.Item<MinorHealingPotion>() ||
			itemModel == ModelDB.Item<MinorManaPotion>() ||
			itemModel == ModelDB.Item<MinorPowerPotion>() ||
			itemModel == ModelDB.Item<MinorStaminaPotion>() ||
			itemModel == ModelDB.Item<MinorCurePotion>();

		private static ItemModel GetAMajorVersionOfAMinorPotion(ItemModel itemModel)
		{
			if(itemModel == ModelDB.Item<MinorHealingPotion>())
			{
				return ModelDB.Item<MajorHealingPotion>();
			}
			else if(itemModel == ModelDB.Item<MinorManaPotion>())
			{
				return ModelDB.Item<MajorManaPotion>();
			}
			else if(itemModel == ModelDB.Item<MinorPowerPotion>())
			{
				return ModelDB.Item<MajorPowerPotion>();
			}
			else if(itemModel == ModelDB.Item<MinorStaminaPotion>())
			{
				return ModelDB.Item<MajorStaminaPotion>();
			}
			else if(itemModel == ModelDB.Item<MinorCurePotion>())
			{
				return ModelDB.Item<MajorCurePotion>();
			}
			else
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		protected override void CalculatePriceApplyFunction(BetweenScenariosEvents.CalculateItemSellPrice.Parameters parameters)
		{
			if(IsMinorPotion(parameters.ItemModel) && GetAMajorVersionOfAMinorPotion(parameters.ItemModel).ShopCount > 0)
			{
				parameters.AdjustSellPrice(-parameters.SellPrice);
			}
		}

		protected override void ItemSoldApplyFunction(BetweenScenariosEvents.ItemSold.Parameters parameters)
		{
			if(IsMinorPotion(parameters.ItemModel))
			{
				ItemModel majorPotionModel = GetAMajorVersionOfAMinorPotion(parameters.ItemModel);

				if(majorPotionModel.ShopCount == 0)
				{
					return;
				}

				parameters.Seller.AddItem(majorPotionModel);
				AppController.Instance.CampaignSaveData.SavedCampaign.GetSavedItem(majorPotionModel).AddUnlocked(1);
				AppController.Instance.CampaignSaveData.SavedCampaign.GetSavedItem(majorPotionModel).RemoveStock(1);

				Complete();
			}
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "See what new potion she's selling.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask to see the new potion and she pushes one forward to you. "Here, take a look. This new potion will turn out to be a great investment! Crafted by the finest alchemists in the land."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemReward(ModelDB.Item<AlchemyPotion>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Ask to exchange one of the potions in your possession.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask to exchange one of your potions and she winks at you. "Normally I'd charge ten gold, but for you honey, it's free today. Let me know which one you'd like to swap out and I'll take care of it right away."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBDowntimeShopExchangeReward()
		];
	}
}