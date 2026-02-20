using System.Collections.Generic;

public class Road10 : RoadEventModel<Road10.ChoiceA, Road10.ChoiceB>
{
	public override int Number => 10;

	public override string Text =>
		"""
		As you pass by a pile of Vermling corpses on the side of the road, you can't help but think there must be some kind of Vermling pandemic going around. You've seen several heaps of dead Vermlings on this journey so far.

		All of a sudden, you feel sharp claws dig into your leg. You look down and see a furry arm sticking out from the pile of corpses, and its paw is tightly gripped around your ankle. The Vermling it belongs to trembles as it uses your leg to slowly pull itself out from the bottom of the pile. With all the strength it could muster, it slowly chokes out the words, "tree... of death... Vermling... gardens..."
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Ignore the message and dig through the pile of corpses for loot.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You search through the corpses, hoping to find something of value, but there is nothing worth scavenging besides rotten flesh.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Ask the Vermling for more information.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask the Vermling to point you in the direction of the gardens. As it draws its final breath, the Vermling scratches coordinates into the dirt before collapsing to the ground.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO:new UnlockScenarioEventReward(ModelDB.Scenario<Scenario046>())
		];
	}
}