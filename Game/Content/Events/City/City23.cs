using System.Collections.Generic;

public class City23 : CityEventModel<City23.ChoiceA, City23.ChoiceB>
{
	public override int Number => 23;

	public override string Text =>
		"""
		With another job done, you make to leave the Traveler's District when you're interrupted by a peculiar sight. You spy a clockwork servant, a bronze-stamped machine of an almost person-like shape - though its body is too large and its limbs too thin - delivering a package to one of the many mansions located here. Odd, since these metal creatures are built and employed by the Artificers, and those know-it-alls rarely let their inventions leave the University grounds, much less unattended. The creature turns its head, a thing shaped like a tool box, from one side and then to the other, as if it were looking for something.

		Then with a sudden jerking the gangly creature takes off at a loping rung, heading to the walls. You get the sense it's fleeing - is it trying to escape?
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Chase after the thing to stop it.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You corner the metal miscreant against the outer walls of the city. It turns, as if noticing you for the first time, when it suddenly lunges. You grapple its arms easily when a third one made of wicked metal protrudes from its body, unfolding from some hidden compartment.

			You try to throw the thing away but it holds you fast, its intent clear. Then suddenly the mechanical monstrosity shudders as it sparks fly from its backside, slumping forward, cold and quiet. A Quatryl in a blue coat approaches. She looks at you, then to the machine, then back at you and finally says, "Well, that wasn't supposed to happen."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Follow at a safe distance and try to discern its intent.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			Following at a safe distance, you're able to keep an eye on the bronze creature while not being discovered. It stops at the city walls, appearing as best you can tell to be looking to the top. You step out from your hiding spot when the creature finally notices you. It scrabbles the wall, digging rough pits with its strong metal clamps. As quickly as it takes you to decide to nock an arrow, the thing is up and over the wall. You try to make sense of the peculiar situation when a Quatryl in a blue coat approaches you. She looks to the wall, and then to you, and finally says, "Well, that wasn't supposed to happen."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}