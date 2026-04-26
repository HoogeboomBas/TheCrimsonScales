using System.Collections.Generic;

public class Road22 : RoadEventModel<Road22.ChoiceA, Road22.ChoiceB>
{
	public override int Number => 22;

	public override string Text =>
		"""
		Walking amongst a cavernous area, you hear a voice calling from within one of the caves. You approach the sound of the voice and see a rope tied to a tree leading down into the deep cavern. "Please, help!" the voice calls from below. "I'm too tired to pull myself up, can you please help me by pulling up the rope?"

		You peer down into the cavern and see only darkness below. The climber must be far down, and it would take great effort to pull him up.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Assist the climber by pulling up the rope.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You start to pull on the rope with all your might. After several minutes of pulling, a bearded man emerges from the cave. "You saved me!" the man exclaims. "Unfortunately I do not have anything of value with me to repay you with, as my expedition proved to be fruitless. However, I'm sure karma will take care of you."

			The man smiles as he pats your shoulder before he gathers his belongings and begins to walk away, leaving you short of breath.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioDiscardingReward(1),
			new AddCityReward(ModelDB.Event<City33>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Cut the rope and steal the bag he left by the tree.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin to cut the rope and hear the voice cry out from below, "Wait! What are you doing?! Please, no! No!" The climber continues pleading as you furiously begin to cut faster. You hear the rope snap, followed by a fading scream and then - silence.

			You pick up his bag with the hopes to sell the valuables within during your next trip to town, but the bag turns out to be filled with nothing but snacks and some old rope.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}