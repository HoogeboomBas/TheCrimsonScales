using System.Collections.Generic;

public class City21 : CityEventModel<City21.ChoiceA, City21.ChoiceB>
{
	public override int Number => 21;

	public override string Text =>
		"""
		You are studying a book in the University library when a black imp suddenly leaps onto your books and snarls in your face. Surprised, you stand up from your chair and look around. The library seems to be empty of occupants but there are black imps everywhere. Some are pulling books from the shelves, others are tearing up and eating the pages.

		The librarian has passed out in her chair, and a black imp is jumping on her head and laughing maniacally as it pulls clumps of hair out from her hair and eats them. It's utter chaos and you have no idea how this infestation occurred.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Try to kill as many black imps as you can.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin to slay the black imps, but there are too many to count. It seems that for every black imp you kill, another two take its place. Weary from slinging your blade, you eventually leave the establishment with a lingering curiosity as to how this infestation occurred in the first place.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Carry the librarian to a safe place and report this to the proper authorities.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You sling the librarian over your back and make your way out of the library, swinging your blade against the imps as they jump toward you on the path to the exit.

			After reporting the infestation to the authorities, the librarian returns to consciousness and explains that she is the wife of Sir Kenhaven, one of the wealthiest merchants in all of Gloomhaven, and promises to return the favor of escorting her out of the library with a guarded escort of her own.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new DoNotDrawRoadReward()
		];
	}
}