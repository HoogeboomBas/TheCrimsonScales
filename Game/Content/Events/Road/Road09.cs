using System.Collections.Generic;

public class Road09 : CityEventModel<Road09.ChoiceA, Road09.ChoiceB>
{
	public override int Number => 09;

	public override string Text =>
		"""
		You're climbing near a canyon when you hear a cry for help. You follow the voice to a rock formation and discover a Savvas with his leg stuck between two rocks. He cries out to you, explaining that he's been stuck for two days and you're the first one to cross his path. He lets out a large groan as he extends his hand out to you for help.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Help pull the Savvas out from the rock formation.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			With all your might, you manage to pull the Savvas out from the rock formation. He stumbles forward and clutches his throbbing leg, but he's smiling from ear to ear. "Please, take this mantle I've been wearing in return for your kindness," the Savvas says. "Don't worry about me, I'll make my way home."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<MantleOfPurity>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Loot his bags and leave him for stranded.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ignore the Savvas groaning as you proceed to rummage through his bags. You find a peculiar pair of boots along with some rope and a blade. "Wait, please, don't abandon me!" the Savvas begs as you hand him the blade from the bag and leave him to figure out his own solution.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<ShoesOfPhasing>())
		];
	}
}