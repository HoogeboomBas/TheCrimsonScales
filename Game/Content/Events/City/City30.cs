using System.Collections.Generic;

public class City30 : CityEventModel<City30.ChoiceA, City30.ChoiceB>
{
	public override int Number => 30;

	public override string Text =>
		"""
		It's Trading Day at the market and vendors have traveled into the city from all over to show their wares. This would be a good opportunity to stock up on new supplies, or you can take the day to pawn off your old merchandise to the many prospective buyers who will be perusing the stalls.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Spend the day shopping for new supplies.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You spend the day browsing the stalls and find a plethora of discounted merchandise. Today is a great day to shop for new supplies, and there are many great deals to take advantage of.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new DowntimeShopPriceEventReward(
				eventReward =>
					parameters =>
					{
						if(!state.GetCustomValue<bool>(parameters.Buyer.Guid.ToString()))
						{
							parameters.AdjustPrice(-10);
						}
					},
				eventReward =>
					parameters =>
					{
						if(!state.GetCustomValue<bool>(parameters.Buyer.Guid.ToString()))
						{
							state.SetCustomValue(parameters.Buyer.Guid.ToString(), true);
						}
					},
				color =>
					$"During this City Phase, each character may buy one item from the shop for {Icons.Inline(Icons.Coins, color: color)}10 less."
			)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Spend the day selling your old merchandise.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You set up a stall and buyers pour in from all directions. With a bit of effort, you manage to persuade a few customers to agree to purchase your unwanted goods for the full asking price.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new DowntimeShopSellPriceEventReward(
				eventReward =>
					parameters =>
					{
						if(!state.GetCustomValue<bool>(parameters.Seller.Guid.ToString()))
						{
							parameters.AdjustSellPrice(10);
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
					$"During this City Phase, each character may sell one item to the shop for {Icons.Inline(Icons.Coins, color: color)}10 more."
			)
		];
	}
}