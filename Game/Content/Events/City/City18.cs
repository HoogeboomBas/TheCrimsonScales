using System.Collections.Generic;

public class City18 : CityEventModel<City18.ChoiceA, City18.ChoiceB>
{
	public override int Number => 18;

	public override string Text =>
		"""
		New tax proposals on beverages in the Sinking Market have caused protests all over the city. You've tried to avoid them whenever possible, but one evening you see a small crowd holding signs formed outside the entrance to the Sleeping Lion.

		"No taxing drink! Taxes must sink!" the crowd chants. You attempt to make your wya through the heavy crowd and mage to reach the entrance of the tavern, but a large Inox is guarding the door.

		"We're not letting anyone in!" the Inox barks. "No drinking until they remove the taxes."
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Join the protesters in the crowd.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin chanting along with the crowd when a group of city guards arrive to break up the protest. They take down all names and collect a small fine from each protester for being a public nuisance.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new LoseReputationEventReward(1),
			new LoseCollectiveGoldEventReward(5)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Demand the Inox either moves aside or face the consequences.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You demand the Inox move aside but he adamantly refuses. A screaming match ensues, and a group of city guards arrive to break up the protest and mistake your desire for ale for a desire for peace. They commend your efforts to break up the protest as they collect a small fine from each protestor, and one of the city guards slips you a small pouch of coin out of appreciation.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainReputationEventReward(1),
			new GainCollectiveGoldEventReward(5)
		];
	}
}