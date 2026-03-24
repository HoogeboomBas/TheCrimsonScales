using System.Collections.Generic;

public class City05 : CityEventModel<City05.ChoiceA, City05.ChoiceB>
{
	public override int Number => 05;

	public override string Text =>
		"""
		You are selling your wares in the market when a Harrower approaches, dressed in golden robes and an intricately designed mask.

		The Harrower begins to chitter and hiss, pointing to your wares as it plops a large pouch on the table.

		You peer inside the pouch and see jewels of various sizes glittering within. The Harrower demands your entire lot of wares in exchange for the bag of jewels.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Firmly demand the Harrower pay in the only currency you accept - gold coin.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You demand the Harrower pay you in gold coin, and it hastily snatches the bag of jewels and proceeds to turn away. The rest of the day passes with no other customers visiting your stall. You return home at the end of the day, wondering what could have been.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Sell the Harrower your lot, and hope the jewels are genuine.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You exchange your wares for the jewels and head straight to the jeweler to have them evaluated. The jeweler deems them authentic and you head to the Sleeping Lion with full pockets to enjoy an early evening in town.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new DowntimeShopSellPriceEventReward(
				eventReward =>
					parameters =>
					{
						if(!state.GetCustomValue<bool>(parameters.Seller.Guid.ToString()))
						{
							parameters.AdjustSellPrice(parameters.ItemModel.Cost - parameters.SellPrice);
						}
					},
				eventReward =>
					parameters =>
					{
						if(!state.GetCustomValue<bool>(parameters.Seller.Guid.ToString()))
						{
							state.SetCustomValue(parameters.Seller.Guid.ToString(), true);
						}
					},
				color =>
					$"During this City Phase, each character may sell one item to the shop for its full gold value."
			)
		];
	}
}