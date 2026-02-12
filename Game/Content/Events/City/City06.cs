using System.Collections.Generic;

public class City06 : CityEventModel<City06.ChoiceA, City06.ChoiceB>
{
	public override int Number => 06;

	public override string Text =>
		"""
		"Haunted, I tell you! It's haunted!" The Quatryl shivers in fright. This particular Quatryl named Shiela has been frequenting the Sleeping Lion and made your acquaintance. You've come to learn that she recently inherited a mansion on the outskirts of the Traveler's District upon her great-uncle's passing.

		"I'd owe you a great deal if you can pay the mansion a visit and clean the place out of the spooky creatures within," Shiela's eyes light up as she places a bag of coin on the table and slides it in your direction.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Offer to investigate the mansion.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You offer to investigate the mansion and Shiela claps her hands together in excitement, "Amazing! I know I could count on you. That place simply spooks me," she shudders. "Here, let me get you the coordinates and you can be on your way."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO:new UnlockScenarioEventReward(ModelDB.Scenario<Scenario044>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Demand a larger sum for the task.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You agree to investigate the mansion in exchange for a larger sum. "How dare you! I thought we were friends!" Shiela scoffs before standing up and angrily storming out of the tavern.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}
}