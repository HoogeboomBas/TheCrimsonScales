using System.Collections.Generic;

public class Road35 : RoadEventModel<Road35.ChoiceA, Road35.ChoiceB>
{
	public override int Number => 35;

	public override string Text =>
		"""
		You come across a group of men huddled together on the side of the road. They seem intensely focused on something as they mutter under their breath while exchanging colorful concoctions and pour various smoking liquids into a pot.

		"Come, travelers! Come see this marvelous experiment!" one of the men gleefully explains as he motions you forward. "You seem like a strong, capable group. We could use some help stirring and mixing the heavy brew."

		You glance over and see a bubbling cauldron with sparks flying all over.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Help the men mix the brew.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin to slowly mix the brew as the men shout in excitement and continue pouring in various liquids. All of a sudden, the pot explodes and the liquid erupts with the force of a volcano. You try and step away as fast as you can, but your clothing becomes singed and your skin is scathed.

			The men begin apologizing and mention something about disproportionate measurements. These men clearly did not know what they were doing.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Wound1)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Decline and continue forward.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You politely decline and carry forward with your journey. Several minutes after departing from the scene, you hear a large explosion and see a cloud of smoke rising in the distance. It's a good thing you didn't stick around.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}
}