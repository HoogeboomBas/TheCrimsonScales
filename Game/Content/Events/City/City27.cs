using System.Collections.Generic;

public class City27 : CityEventModel<City27.ChoiceA, City27.ChoiceB>
{
	public override int Number => 27;

	public override string Text =>
		"""
		You're perusing through the market when a Vermling Shaman approaches you with one trinket in each hand. "Blessed be the one who chooses faith, cursed be the one who chooses fortune," the Shaman rambles. "Blessed be the one who chooses prosperity, cursed be the one who chooses wealth."

		"Which will you choose?" the shaman lifts up the trinkets to reveal two amulets dangling from their chains. "The amulet of security or the amulet of restoration?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Choose the amulet of security.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You point to the amulet of security and a frown falls upon the Vermling's face. "You have been warned! Curses upon you!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionReward(Conditions.Curse)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Choose the amulet of restoration.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You point to the amulet of restoration and a smile forms upon the Vermling's face. "You shall be blessed! Blesses upon you!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			//TODO: You may choose to ignore the effects of the next road event drawn.
			new DoNotDrawRoadReward()
		];
	}
}