using System.Collections.Generic;

public class City35 : CityEventModel<City35.ChoiceA, City35.ChoiceB>
{
	public override int Number => 35;

	public override string Text =>
		"""
		That time of year has come around again. The Annual University Faire is on display today near the Market, where many come from far and wide to witness the latest technological and scientific accomplishments.

		Fully open to the public, some attend to contribute towards investing in different technologies and weaponry while others simply peruse and marvel at the various medical advancements.

		The convention only runs for one day, so you have to decide how to spend your time.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Head to the technology section to scout for new weaponry.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You head straight to the technology section of the fair and spend hours browsing through the impressive displays. Towards the end, a Quatryl motions you forward and places a pair of odd looking shoes on the table.

			"We brought too much inventory and can't bring it all back to our homeland," the Quatryl explains. "Help yourself to these."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<ComfortableShoes>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Visit the science booths to inspect new medical developments.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You spend the day perusing the science section of the fair, marveling at the various potions and medical devices. Towards the end, a smiling man beckons you towards his direction.

			"We brought an assortment of health potions to showcase here today. However, we brought one too many and decided to give out the remaining for free. If you care for one, feel free to take the last one."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<MajorHealingPotion>())
		];
	}
}