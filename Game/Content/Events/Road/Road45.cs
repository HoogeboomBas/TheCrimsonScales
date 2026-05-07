using System.Collections.Generic;

public class Road45 : RoadEventModel<Road45.ChoiceA, Road45.ChoiceB>
{
	public override int Number => 45;

	public override string Text =>
		"""
		It's dark at night and you're traveling near the shorelines when your lamp unexpectedly burns out. Realizing you have not brought enough oil to rekindle the light, you cautiously continue on with your journey when you notice a glowing light source up ahead.

		As you near the light source, you find yourself face-to-face with a giant glowing Lurker. You carefully approach the creature and see a sign posted nearby that reads, 'LUMINARY FOR HIRE: 10 GOLD.'
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Pay the Lurker to escort you on your journey.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			The Lurker happily accepts your sum and accompanies you on the journey, lighting the path ahead.

			When you reach your destination, it doesn't want to leave your side. It seems it's here to stay, and you realize you haven't just hired a light source... you hired a mercenary.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new LoseCollectiveGoldReward(10),
			new SpawnReward(ModelDB.Monster<Lurker>(), 4)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Save your hard-earned coin and take your chances in the dark.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You continue on with your journey, making your way off the shoreline and into a forest when you accidentally stumble into a thorn-bush. Bruised and cut, you finally reach your destination and vow never to travel without extra oil again.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithDamageReward(2)
		];
	}
}