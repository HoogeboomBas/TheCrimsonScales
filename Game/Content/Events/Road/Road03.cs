using System.Collections.Generic;

public class Road03 : RoadEventModel<Road03.ChoiceA, Road03.ChoiceB>
{
	public override int Number => 03;

	public override string Text =>
		"""
		There is a faint screaming echoing from the distance. As you approach closer, you see a deep pit dug on the side of the road. There is a small Vermling in there who must have fallen in, and the pit is filled with snakes and scorpions.

		The Vermling seems frightened and softly whimpers as it looks up to you for help. You feel compelled to do something for the poor Vermling.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Help the Vermling out of the pit.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You extend your hand down to the Vermling and quickly pull it up as scorpions snap toward it. Had you come a few minutes later, it would've been too late.

			The Vermling looks up to you gratefully with watery eyes before hurriedly scurrying away.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainXPEventReward(5)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Throw the Vermling a few morsels for food.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You grab a loaf of bread from your bag and throw it down into the pit. As the Vermling reaches toward it, the snakes in the pit hiss and extend their fangs. You watch in horror as the snakes indulge in their meal, leaving the bread untouched. You can't help but feel a tinge of remorse as you turn and slowly walk away.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Curse)
		];
	}
}