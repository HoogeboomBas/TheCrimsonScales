using System.Collections.Generic;

public class City13 : CityEventModel<City13.ChoiceA, City13.ChoiceB>
{
	public override int Number => 13;

	public override string Text =>
		"""
		You impatiently tap your foot as you stand by the door within Shiela's potion shop. She was supposed to meet you today to show you her new wares, but you found the shop empty when you arrived an hour ago.

		As you contemplate leaving, she bursts through the door in a hurried manner. "I'm so sorry!" she exclaims as she catches her breath. "That shouldn't have taken so long."

		"Please, accept my apologies. I know your time is valuable. Please allow me to compensate you for lost time."

		"How could I make it up to you? Did I cause you to miss out on anything?"
		""";

	public class ChoiceADowntimeShopPriceReward : DowntimeShopPriceReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"The next {Icons.Inline(Icons.GetItem(ItemType.Small), textParameters)} item bought this City Phase will be half price (rounded down).";

		protected override void CalculatePriceApplyFunction(BetweenScenariosEvents.CalculateItemBuyPrice.Parameters parameters)
		{
			if(parameters.ItemModel.ItemType == ItemType.Small)
			{
				parameters.AdjustPrice(-parameters.Price / 2);
			}
		}

		protected override void ItemBoughtApplyFunction(BetweenScenariosEvents.ItemBought.Parameters parameters)
		{
			if(parameters.ItemModel.ItemType == ItemType.Small)
			{
				Complete();
			}
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Tell Shiela that time is money, and she needs to pay up.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell Shiela that time is money, and she blushes as tears swell up in her eyes. "I'm terribly sorry, but business has been slow," she stammers as she opens her cash register to reveal it to be completely empty. "I'm afraid I have no gold at the moment, but I can offer you a discount on a potion instead."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceADowntimeShopPriceReward()
		];
	}

	public class ChoiceBDowntimeShopPriceReward : DowntimeShopPriceReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"The next {Icons.Inline(Icons.GetItem(ItemType.Small), textParameters)} item bought this City Phase with a value of {Icons.Inline(Icons.Coins, textParameters)}30 or less will be free.";

		protected override void CalculatePriceApplyFunction(BetweenScenariosEvents.CalculateItemBuyPrice.Parameters parameters)
		{
			if(parameters.ItemModel.ItemType == ItemType.Small && parameters.ItemModel.Cost <= 30)
			{
				parameters.AdjustPrice(-parameters.Price);
			}
		}

		protected override void ItemBoughtApplyFunction(BetweenScenariosEvents.ItemBought.Parameters parameters)
		{
			if(parameters.ItemModel.ItemType == ItemType.Small && parameters.ItemModel.Cost <= 30)
			{
				Complete();
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Assure Shiela she shouldn't worry about it; you had no other plans anyhow.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell Shiela not to worry about it, and she blushes. "Nonsense! You're a great friend, and it's completely my fault. At the very least, let me offer you one of my potions at no cost."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBDowntimeShopPriceReward()
		];
	}
}