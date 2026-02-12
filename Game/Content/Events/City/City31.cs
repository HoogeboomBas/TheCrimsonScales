using System.Collections.Generic;

public class City31 : CityEventModel<City31.ChoiceA, City31.ChoiceB>
{
	public override int Number => 35;

	public override string Text =>
		"""
		You are spending the day perusing the large, opulent mansions in the Traveler's District when you come across Sir Kenhaven's mansion.The vast size of the house doesn't surprise you, built complete with many servants' quarters and lavish gardens. After all, he is one of the most wealthy Valrath in all of Gloomhaven.

		"Hey you! What are you doing here?!" a voice calls out from behind. You turn around to see two guards looking sternly toward you. They must be here to protect the grounds.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Explain you must have gotten lost.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You explain that you must have gotten lost on your way. "Leave here at once," one of the guards snarls. "This is no place for lost fools."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Request a tour of the mansion.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask the guards for a tour of the mansion, and before they could respond you hear a voice call out from behind. "They're with me!"

			Sir Kenhaven himself appears and the guards quickly move out of his way. "Welcome to my mansion grounds. As an adventurer, I'm sure you'd appreciate the labyrinth maze I put together. There's a great reward for anyone who solves the puzzle and escapes. Care to see it?"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO:new UnlockScenarioEventReward(ModelDB.Scenario<043>())
		];
	}
}