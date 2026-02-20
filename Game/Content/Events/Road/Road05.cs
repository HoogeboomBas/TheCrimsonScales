using System.Collections.Generic;

public class Road05 : RoadEventModel<Road05.ChoiceA, Road05.ChoiceB>
{
	public override int Number => 05;

	public override string Text =>
		"""
		The music gets louder as you near a large tent on the side of the road, marked with a multitude of Inox tribal signs. You hear drums banging and cheering from within while shadows of Inox figures dance around in a circle.

		It sounds like a celebration of dance and song, but something smells like it's burning and there is smoke pouring out from various holes in the tent.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Approach the tent and demand to know what's cooking.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You approach the tent and are greeted by an Inox adorned in piercings and jewelry made of teeth. You ask her what's burning in the tent, and she hands you a bowl of hot soup, explaining that she's been preparing the meat all day. Exhausted from a long day's journey, you thank the Inox and hungrily shovel the chunky soup into your mouths; it has a peculiar yet palatable flavor. You hand the empty bowl back to her while once more expressing your gratitude.

			On your way back to the road, you watch a second Inox dumping out a large pot filled with congealed grease and what appears to be human bones. Your stomach grumbles unpleasantly, and you feel ill.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainXPEventReward(3),
			new AllStartScenarioWithDamageEventReward(3)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Request to join them in song and dance.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You enter the tent and join hands with the Inox as you sing and dance around a fire. There is a pot bubbling in the center of the fire, and the Inox take turns pausing the dancing to throw in bones while chanting before erupting into more song and dance.

			You've had a great time, but nightfall is approaching and it's time to carry on with your journey. The Inox give you gifts to depart with as you excuse yourself from the event. 
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(5),
			new GainCollectiveItemEventReward(ModelDB.Item<NecklaceOfTeeth>())
		];
	}
}