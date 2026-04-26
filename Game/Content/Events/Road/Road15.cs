using System.Collections.Generic;

public class Road15 : RoadEventModel<Road15.ChoiceA, Road15.ChoiceB>
{
	public override int Number => 15;

	public override string Text =>
		"""
		It's the middle of the day and you reach a fork in the road. On the left, you see a path leading into a dark forest. There are various insects buzzing beneath the trees. On the right, there is a straight dirt path laden with thorns.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Take the left path through the forest.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You head into the forest but the density of the trees blocks the sunlight and you have trouble staying on the path. In the midst of the darkness you accidentally stumble into a large beehive and eventually make it out of the forest with a few burning stings to remember your journey by.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionReward(Conditions.Wound1),
			new AllStartScenarioWithDamageReward(2)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Take the right path through the thorns.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			As the sun shines down from above, you walk through the dirt path and are easily able to avoid the thornbushes in the daylight. You manage to make it through the path without a single scratch.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainXPReward(6)
		];
	}
}