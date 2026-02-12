using System.Collections.Generic;

public class Road29 : CityEventModel<Road29.ChoiceA, Road29.ChoiceB>
{
	public override int Number => 29;

	public override string Text =>
		"""
		You happen across a Quatryl stranded on the side of the road as if waiting to hitch a ride. "Hey, I recognize you!" the Quatryl peps up. "It's me, Shiela!"

		"I've gotten kind of lost after last night's drinking. If you're heading towards the Mixed District, do you mind letting me tag along?"

		You weren't planning on heading in that direction, but Shiela's a good friend.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Go out of your way and escort Shiela to the Mixed District.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You escort Shiela back to the Mixed District, and she thanks you profusely before heading into her apartment. It was an uneventful journey, but you find yourself far behind schedule as this was much out of your way.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioSpendingItemTypeEventReward(ItemType.Feet),
			new AddCityEventEventReward(ModelDB.Event<City32>())
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Tell Shiela she'll have to find another ride and continue on your journey.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"Please!" Shiela begs as you adamantly refuse to go out of your way. She bursts into tears as you walk away, shouting after you, "The other patrons in the Sleeping Lion will surely hear about this!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new LoseReputationEventReward(2)
		];
	}
}