using System.Collections.Generic;

public class City14 : CityEventModel<City14.ChoiceA, City14.ChoiceB>
{
	public override int Number => 14;

	public override string Text =>
		"""
		You're about to order another glass of ale in the Sleeping Lion when suddenly a bird flies in through the open door. The bartender grabs a broomstick and begins chasing the bird around, leaving the tab unattended. 
		""";

	public class ChoiceADowntimeShopPriceReward : DowntimeShopPriceReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"The next item purchased this City Phase will cost {Icons.Inline(Icons.Coins)}10 less.";

		protected override void CalculatePriceApplyFunction(BetweenScenariosEvents.CalculateItemBuyPrice.Parameters parameters)
		{
			parameters.AdjustPrice(-10);
		}

		protected override void ItemBoughtApplyFunction(BetweenScenariosEvents.ItemBought.Parameters parameters)
		{
			Complete();
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Help the bartender chase the bird out of the tavern.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You grab a mop from nearby and help the bartender guide the bird out of the open door. Grateful for you assistance, the bartender offers you free drinks for the remainder of the night. The coin you saved will go a long way.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceADowntimeShopPriceReward()
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Take the opportunity to sneak a free drink from the tap.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You quickly place your mug under the tap and begin to fill the glass with ale. As you top off the drink, you look up to see the bartender standing over you with his arms crossed and eyes narrowed. You attempt to guzzle the ale as fast as you can before being forcefully escorted out of the tavern.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new LoseReputationReward(1)
		];
	}
}