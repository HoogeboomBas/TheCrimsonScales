using System.Collections.Generic;

public class Road01 : RoadEventModel<Road01.ChoiceA, Road01.ChoiceB>
{
	public override int Number => 01;

	public override string Text =>
		"""
		After having misread your map, you find yourself lost deep in the Dagger Forest and are startled when you come across an Orchid riding atop a massive sabretooth tiger. The Orchid approaches you as you draw your blade, but she peacefully dismounts the tiger and warns you to turn around.

		She claims that a mysterious force has taken hold of some of the animals in this part of the forest, and her tribe is on a mission to be rid of the beasts that are terrorizing their village.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Heed her warning and ask for directions to your destination.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask the Orchid for directions and she points you toward the right path. You wish her well with her endeavors and reach your destination in peace.
			""";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Help the Orchid on her mission to kill the terrorizing beasts.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You offer to help the Orchid on her mission and her face lights up with contentment. She relays the coordinates of where to find the beasts and wishes you luck as she excuses herself to tend to her tribe.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new UnlockScenarioReward(ModelDB.Scenario<Scenario042>())
		];
	}
}