using System.Collections.Generic;

public class City09 : CityEventModel<City09.ChoiceA, City09.ChoiceB>
{
	public override int Number => 09;

	public override string Text =>
		"""
		There is a talent show tonight in the Brown Door and you've decided to sit in for an evening of fun and laughs.

		Halfway through the show, you are suddenly shocked to hear your name being called to the stage. Evidently, the sign-in sheet at the front entrance wasn't for guests, but for performers.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Sneak out the back door, you're not prepared for this.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			They continue calling your name as you slink out the back door. Looks like they'll have to find another patron to fill the slot.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new LoseReputationEventReward(1)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Go on stage to perform, you have a trick or two up your sleeve.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You go on stage and begin singing and dancing. A nearby Quatryl tosses you a few balls and you begin juggling them as you dance. The crowd begins clapping along to your singing and your act is followed by a roaring applause.

			You're pleasantly surprised when you hear your name being called up as one of the winners of the show.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO: Add reward to get one +1 enhancement to any level 1 or X card.
		];
	}
}