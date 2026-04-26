using System.Collections.Generic;

public class City16 : CityEventModel<City16.ChoiceA, City16.ChoiceB>
{
	public override int Number => 00;

	public override string Text =>
		"""
		Rumors of an oversized Vermling monstrosity living in the sewers have been circulating around the city. After a few drinks one night in the Sleeping Lion, you accept a dare to investigate the matter firsthand.

		You enter the sewers with a lamp and wade around in the muck. After an hour of searching, you prepare to give up and return to the tavern when you suddenly hear scratching noises coming from the pipes ahead. You head toward the sounds and find yourself face-to-face with a Vermling bigger than any you've ever seen.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Run back to the tavern to tell the other patrons about the sight.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You return to the tavern with claims of having witnessed the gigantic Vermling, but the rest of the patrons laugh off your story and consider your tale to be brought upon by too much ale.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new LoseReputationReward(1)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Attempt to capture the Vermling for proof.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You charge toward the Vermling and attempt to grab it ut it snarls loudly as it runs off. You're in no condition to give chase, but you managed to grab a clump of fur.

			You return to the tavern and showcase your findings. The bartender places the fur in a jar as a trophy and the other patrons offer you a round of drinks in victory.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AddRoadReward(ModelDB.Event<Road34>())
		];
	}
}