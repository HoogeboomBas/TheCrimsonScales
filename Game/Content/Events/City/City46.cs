using System.Collections.Generic;

public class City46 : CityEventModel<City46.ChoiceA, City46.ChoiceB>
{
	public override int Number => 46;

	public override string Text =>
		"""
		You're enjoying the evening attending a luminary light show, where bioluminescent Lurkers known as 'Luminaries' perform by dancing and demonstrating different light patterns. These shows are often sold out, but you were lucky enough to buy one of the last tickets available.

		Halfway through the show, one of the Luminaries onstage suddenly begins changing colors at rapid speed and raises its claws in the air. It jumps into the crowd and snaps at the audience, who begin screaming and running in all directions. The Lurker seems to have lost its mind as it grabs a little girl by the hair and flings her across the room.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Attempt to subdue the Lurker with physical force.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You rush to the Lurker and attempt to tackle it, but it throws you off its back with great force. As you fall to the ground, several Inox carrying a battering ram call toward you. You jump to your feet and take hold of the battering ram as the Inox count down from three before you all charge together toward the Lurker. The Lurker is knocked back from the battering ram and is instantly rendered unconscious. The Inox proceed to carry the Lurker away, leaving the battering ram behind. 
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<BatteringRam>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Run to the little girl to make sure she gets to safety unharmed.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You run to the little girl as you hear a woman crying out, "Help! My daughter!" You scoop the girl up into your ams and carry her to her mother, who tearfully thanks you in the midst of the calamity. You turn back to see the Lurker unconscious in the arms of several Inox who appear to be hauling it out of the arena.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainReputationEventReward(2),
			new GainProsperityEventReward(1)
		];
	}
}