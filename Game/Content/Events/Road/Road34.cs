using System.Collections.Generic;
using System.Linq;

public class Road34 : CityEventModel<Road34.ChoiceA, Road34.ChoiceB>
{
	public override int Number => 34;

	public override string Text =>
		"""
		Walking through a mountainous area not too far from the city, you double-take as you realize what you first thought to be a sleeping bear on the side of the road is a gargantuan Vermling. You recognize the creature from the time you investigated the sewers in search of a giant Vermling and vow not to let it get away from you a second time.

		It snores loudly and turns over as you slowly approach it from behind. It's in deep slumber and you must decide how to proceed.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Set a bear trap near the Vermling and wait.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You set up a bear trap near the Vermling and eagerly wait for it to awaken. After what seems like hours, the Vermling finally stands up and you hear a loud snap. You pop out of the bushes, only to find both the Vermling and the trap to have disappeared. You groan upon realizing that it'll take more than a bear trap to subdue this beast, and hope to encounter it again in the future.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Ambush it with an attack while it sleeps.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You sneak up toward the Vermling and swing at it with your blade. Having been awoken from the gash, the Vermling jolts awake and lunges toward you, claws extended and slashes you across the face before running off into the mountains. You wipe the blood from the wound and recognize that you could have prepared yourself better for the fight.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Wound1)
		];
	}
}