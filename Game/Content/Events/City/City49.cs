using System.Collections.Generic;

public class City49 : CityEventModel<City49.ChoiceA, City49.ChoiceB>
{
	public override int Number => 49;

	public override string Text =>
		"""
		There's a solar eclipse due to take place today, and you decided to join the crowds that have gathered in the fields to witness the spectacular event.

		As the light shifts away, the entire field is blanketed in darkness. You hear a ruffle and a scream, and feel a figure swiftly brush by you. As a sliver of light peeks out from beneath the eclipse, you see an Aesther pushing his way through the crowd as gold coins fall from his pockets, leaving a trail behind.

		"Help! I've been robbed!" you hear a woman scream.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Quietly pick up the scattered coins in the confusion.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You kneel down on the floor and scoop up a pile of coins that fell from the Aesther's pockets. The Aesther runs out of sight as the woman cries hysterically. You look around to ensure there are no witnesses as you shuffle yourself back into the crowd.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(15)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Run after the Aesther.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You dash after the Aesther and tackle them to the ground. Coins scatter everywhere, and as the light begins to increase, a crowd forms a circle around you.

			City guards arrive to arrest the Aesther, and proceed to explain there was a bounty reward on this thief.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainReputationEventReward(2),
			new GainCollectiveGoldEventReward(20)
		];
	}
}