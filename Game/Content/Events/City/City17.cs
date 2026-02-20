using System.Collections.Generic;

public class City17 : CityEventModel<City17.ChoiceA, City17.ChoiceB>
{
	public override int Number => 17;

	public override string Text =>
		"""
		You're perusing the alchemy shops in the Mixed District when you happen upon a storefront window and see a Quatryl you're familiar with named Shiela. She smiles cheerfully as you enter the store and she places several potions on the table.

		"Care to buy one of our newest potions?" she glees. Before you can answer, she pulls out another set of potions from the wall behind her. "Otherwise we have a new program where you can exchange old potions out for new."
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "See what new potion she's selling.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask to see the new potion and she pushes one forward to you. "Here, take a look. This new potion will turn out to be a great investment! Crafted by the finest alchemists in the land."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<AlchemyPotion>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Ask to exchange one of the potions in your possession.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask to exchange one of your potions and she winks at you. "Normally I'd charge ten gold, but for you honey, it's free today. Let me know which one you'd like to swap out and I'll take care of it right away."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO: One player may return any Minor potion from their possession to the shop and receive a Major potion of the same type for free
		];
	}
}