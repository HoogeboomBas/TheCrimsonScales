using System.Collections.Generic;

public class Road56 : RoadEventModel<Road56.ChoiceA, Road56.ChoiceB>
{
	public override int Number => 56;

	public override string Text =>
		"""
		As you round a turn in the road, you're surprised to see two Savvas locked in combat, circling each other.

		One of them you recognize as your former Hollowpact companion. The other figure appears to be a younger but similarly accoutred Savvas. Both have glowing amethyst stones filling their chest cavities with an otherworldly light.

		The two Hollowpact are undeterred by your presence and continue to circle each other; searching for an opening. Both are lithe and move with a duelist's grace despite their rocky forms - a notable difference in style from many of their Savvas kin. Your former companion turns slightly and addresses you, "This one is mine. Stay out of this. Won't ask twice."
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Try to calm the situation. There's no need for this violence.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			The younger Hollowpact takes advantage of your interruption to teleport away. Your former ally ignores this and turns to you, its voice grating like jagged rocks against metal, "You. DON'T understand. We are abonimations. Must die. Must ALL die."

			A moment passes and rage is instantly replaced by calm. The Hollowpact speaks again with a ponderous, gravelly voice, "I can track and destroy my quarry easily enough. I save this young one from itself. Stop me again and die." With that, the Hollowpact vanishes.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Listen to your former ally. Watch the events unfold without interference.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			Once it becomes clear you won't intervene, the two Hollowpact return their focus to each other. The younger strikes first, lashing out with a devastating flurry.

			The elder Savvas dodges several blows and then, in between incoming strikes, grabs the glowing stone in the chest of the younger fighter and shatters it in a burst of energy. The younger Hollowpact's eyes go wide as it begins to disintegrate from the inside out. Your former ally nods, kicks the slain Savvas's coin purse to you, and vanishes.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldReward(12)
		];
	}
}