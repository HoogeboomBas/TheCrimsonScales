using System.Collections.Generic;

public class City42 : CityEventModel<City42.ChoiceA, City42.ChoiceB>
{
	public override int Number => 42;

	public override string Text =>
		"""
		As you make your way through the bustling market, you notice an old friend, the Hierophant, arguing with a group of Inox.

		"These Inox defy the very existence of the Great Oak!" the Hierophant throws up his hands in anger. "They refuse to relinquish their territory in the far western edge of the Dagger Forest for our new outpost. We will not be stopped!"

		The Inox look to you, teeth gritting and fists clenched tightly. "We'll never give up our land."
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Side with the Hierophant";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You take the side of the Hierophant, much to the dismay of the Inox. "Thank you, dear friends!" the Hierophant beams. "Your allegiance shall be rewarded. We shall meet again soon!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO:new UnlockScenarioEventReward(ModelDB.Scenario<Scenario049>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Side with the Inox";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You take the side of the Inox and the Hierophant shakes his fist and yells, "You shall regret this!"

			The Inox shake your hand as they thank you for your support and provide you with the coordinates of their campground.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO:new UnlockScenarioEventReward(ModelDB.Scenario<Scenario050>())
		];
	}
}