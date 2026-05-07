using System.Collections.Generic;

public class Road11 : RoadEventModel<Road11.ChoiceA, Road11.ChoiceB>
{
	public override int Number => 11;

	public override string Text =>
		"""
		Walking along a dirt path, you come across a small Aesther pinned down on the ground by a giant bear. The Aesther is whimpering with bear drool dripping down his cheek as the bear looks up to you and roars.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Attempt to rescue the Aesther by fighting the bear.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You charge toward the bear with your weapon and give out a battle cry. The bear stands up and swipes in your direction, but you manage to dodge the attack and plunge your weapon into the bear's chest. Startled and wounded, the bear staggers off into the woods.

			"I am forever grateful," the Aesther bows toward you as he dusts himself off. "I am Nerro, the noble dream interpreter who practices in the Ward of Scales. Come see me if you ever need a dream interpreted."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AddCityReward(ModelDB.Event<City34>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Distract the bear by throwing it food in the other direction.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You throw food in the other direction, hoping to distract the bear. You remain hopeful as the bear stands up and sniffs the air, but your hopes are then crushed when the bear leans back over the Aesther and smacks its lips. You turn away as the bear opens its jaws wide and proceeds to indulge in its tasty meal.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}