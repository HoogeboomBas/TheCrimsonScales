using System.Collections.Generic;

public class City32 : CityEventModel<City32.ChoiceA, City32.ChoiceB>
{
	public override int Number => 32;

	public override string Text =>
		"""
		You decide to go shopping in the Mixed District. As the day winds down, you find yourself with enough time to visit one more shop before they close for the day.

		You are tempted to enter Shiela's potion shop, but a new armory across the path catches your attention.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Visit Shiela's potion shop.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You decide to enter Shiela's potion shop and she embraces you as you walk through the door. "Thank you so much for getting me home safe the other day!" Shiela beams with enthusiasm. "Here, we just got a batch of new potions in the morning. Take one, on me!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<IntoxicatingPotion>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Visit the new armory.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You decide to head into the armory and are greeted by a tall Valrath wearing light plate armor. He happily escorts you into the shop and proceeds to show you his latest wares.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainItemDesignEventReward(ModelDB.Item<WovenPlateArmor>()),
			new GainItemDesignEventReward(ModelDB.Item<MantleOfPurity>()),
		];
	}
}