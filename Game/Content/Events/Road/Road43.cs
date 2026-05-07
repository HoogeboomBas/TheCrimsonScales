using System.Collections.Generic;

public class Road43 : RoadEventModel<Road43.ChoiceA, Road43.ChoiceB>
{
	public override int Number => 43;

	public override string Text =>
		"""
		You happen upon a sleeping hooded figure tucked upon a pile of straw on the side of the road. As you peer closer, you begin to take notice that the face looks vaguely familiar. You've seen 'wanted' posters around town with a similar-looking face. Perhaps this is a wanted bandit, or perhaps she's just a lawfully abiding citizen taking rest from her journey...
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Tie her hands together while she sleeps.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin to slowly tie rope around her hands when her snoring comes to an abrupt halt. "What are you doing?!" she exclaims. As you explain you're there to collect her bounty and won't leave without a fight, she retrieves identification and proves you mistaken. She curses you for disturbing her slumber and rolls back into the straw.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionReward(Conditions.Curse)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Awaken and interrogate the figure.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You shake the girl awake and demand identification. She angrily shows you her identification, proving you mistaken.

			After a bit of conversation, you find out she's been separated from her group and you invite her to join you on your journey.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new SpawnReward(ModelDB.Monster<BanditArcher>(), 3)
		];
	}
}