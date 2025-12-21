using System.Collections.Generic;

public class Road21 : CityEventModel<Road21.ChoiceA, Road21.ChoiceB>
{
	public override int Number => 21;

	public override string Text =>
		"""
		You come across a group of Brightsparks on the side of the road. Dressed in full lab gear, they beckon you toward them and offer you two vials to choose from. One of the vials is filled with a glowing red liquid and the other with a bubbling green ooze.

		"We're running an experiment and would appreciate if you would test one of these out for us," the Brightspark grins. "Just let us know how it goes when you make your way back to the city."
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Try the glowing red liquid.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You guzzle the glowing red liquid, and it has a delicious berry taste. Moments after licking the last drop, your head begins to pulse and ache. You stumble away, feeling weary and confused.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Muddle)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Try the bubbling green ooze.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You gulp down the bubbling green ooze, and it has a spicy, bitter aftertaste. Moments after finishing the last drop, you feel a rush of energy surge throughout your body. You turn away with your head held high, feeling invigorated and refreshed.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Strengthen)
		];
	}
}