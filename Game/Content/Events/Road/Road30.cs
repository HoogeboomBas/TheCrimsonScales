using System.Collections.Generic;

public class Road30 : CityEventModel<Road30.ChoiceA, Road30.ChoiceB>
{
	public override int Number => 30;

	public override string Text =>
		"""
		Snow continues to fall fast as you wade through the sleet. This blizzard is unlike anything you've ever experienced, and as you traverse through the barren area you contemplate finding a place to rest through the unexpected storm. You take notice of an isolated cave with a glowing blue light emanating from within, and as you enter the cave you find an Orchid meditating with her eyes closed.

		You recognize the Orchid as a Frostborn; you've heard tales how seeing a Frostborn could be a bad omen, but she seems deep in thought and doesn't take notice of you.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Take respite in the cave.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You spend the night in the cave, taking care not to make any noise so as not to disturb the meditating Frostborn. As you prepare to leave, you feel a cold hand on your shoulder.

			You turn to see the Frostborn, who offers to teach you one of her powerful spells to help you on your journey.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllMonstersStartScenarioWithConditionEventReward(Conditions.Immobilize)
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Continue on and brave the storm.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You depart from the cave and continue on through the snowstorm. You eventually make it to your destination, but your hands are numb from the cold and you can no longer feel you feet.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Immobilize)
		];
	}
}