using System.Collections.Generic;

public class Road14 : RoadEventModel<Road14.ChoiceA, Road14.ChoiceB>
{
	public override int Number => 14;

	public override string Text =>
		"""
		It's the middle of the night and you reach a fork in the road. On the left, you see a path leading into a dark forest. There are various bioluminescent bugs glowing beneath the trees. On the right, there is a straight dirt path laden with thorns.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Take the left path through the forest.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You head into the forest and find that the bioluminescent bugs provide you with a clean, bright light that illuminates the path before you. You take advantage of the light and stride through the path, avoiding several large beehives on the way.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainXPEventReward(6)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Take the right path through the thorns.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You head into the thorn-laden path but find it difficult to navigate around the hazardous bushes in the dark. You end up reaching your destination with a few bleeding cuts after stumbling headfirst into a thornbush.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Wound1),
			new AllStartScenarioWithDamageEventReward(2)
		];
	}
}