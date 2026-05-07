using System.Collections.Generic;

public class City29 : CityEventModel<City29.ChoiceA, City29.ChoiceB>
{
	public override int Number => 29;

	public override string Text =>
		"""
		You're enjoying a glass of cold ale in the Sleeping Lion one night when a group of Inox barge in. You succeed in ignoring their bellowing until one of them approaches your table. The Inox looks back at his friends with a wide grin before clearing his throat. All of a sudden, he grabs your glass of ale and spits a large wad of saliva in your drink before placing it back in front of you.

		His friends laugh and cheer as he flexes his muscles and begins to walk back toward his group.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Pour the ale over his head in retaliation.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You approach the Inox from behind, drink in hand, and pour the remainder of the glass over his head. He turns around to face you, horns dripping with ale, before grumbling to his friends. Seeing you mean business, the group of Inox quickly shuffle themselves out of the tavern, leaving you with a cheering crowd and a table full of untouched drinks to enjoy.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainXPReward(5),
			new GainReputationReward(1)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Quietly order another drink.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You quietly order another drink as the Inox continue their bellowing. The noise gets so loud that you decide to call it an early night and go home to sleep off the incident. The Inox cheer and spit in your direction as you leave the tavern shamefaced.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new LoseCheckmarkReward()
		];
	}
}