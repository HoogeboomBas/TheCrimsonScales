using System.Collections.Generic;

public class City54 : CityEventModel<City54.ChoiceA, City54.ChoiceB>
{
	public override int Number => 54;

	public override string Text =>
		"""
		After a long week of nothing but stale rations on the road, you decide to treat yourself to some good food at The Salty Duck in the Coin District. You heard that your old friend, the Mirefoot, sometimes stops by to sell spices to the chef.

		"Friends!" the Mirefoot calls out as you walk into the restaurant, "you picked the perfect time to visit. I just gave the chef the most amazing red berries that I found on the road. Order the meatballs and you'll be blown away by the flavor of the jam they're served with today."

		You've always trusted the Mirefoot when it comes to plants, but you've had experiences with red berries in the past, and the chef insists the duck today is the best he's ever prepared.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Order the meatballs.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You've never known the Mirefoot to steer you wrong, so despite your own feelings about red berries from the road, you order the meatballs. As promised, the berry jam is the most delicious meal you've ever experienced.

			When you try to pay for your meal, you are told that the Mirefoot already paid your bill before leaving earlier.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionReward(Conditions.Bless, Conditions.Strengthen)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Order the duck.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You normally trust the Mirefoot, but red berries just make you too nervous, and everyone else in the restaurant seems to be thoroughly enjoying the duck.

			The chef was right, the duck was astounding. You pay your bill, and leave feeling well-fed and full of energy for your next adventure.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionReward(Conditions.Strengthen),
			new LoseGoldEachReward(3)
		];
	}
}