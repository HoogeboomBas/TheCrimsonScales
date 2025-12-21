using System.Collections.Generic;

public class Road24 : CityEventModel<Road24.ChoiceA, Road24.ChoiceB>
{
	public override int Number => 24;

	public override string Text =>
		"""
		You're carefully walking along a forest path when you see a set of traps up ahead. They appear to have been marked for animals, but wildlife in these parts is scarce and someone could easily break their foot by accidentally stepping in one of the traps.
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Attempt to dismantle the traps.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin to dismantle each trap, carefully closing them while being mindful of your grip.

			As the last trap shuts closed, you realize one of these could be useful in your journey forward.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<IronSnare>())
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Avoid the traps and proceed to walk through the path.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tiptoe your way around the traps when you hear a loud 'SNAP!' and feel a sharp pain run up your leg. You've accidentally stepped into one of the traps, and it's going to be painful to remove.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithDamageEventReward(2),
			new AllStartScenarioWithConditionEventReward(Conditions.Immobilize)
		];
	}
}