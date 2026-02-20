using System.Collections.Generic;

public class Road57 : RoadEventModel<Road57.ChoiceA, Road57.ChoiceB>
{
	public override int Number => 57;

	public override string Text =>
		"""
		You're drawn toward colorful pillars of smoke pouring out from a small cavern on the side of the road. Curiosity gets the better of you as you wade through the smoke and enter the cavern, only to see a familiar figure standing near a stone covered in chromatic ooze. It's the Brightspark, and he's clapping in delight as he reaches for a vial on the stone beside him.

		"My friends!" he calls toward you with glee. "I've found a secluded place to conduct my experiments, and I must tell you, the elemental converter is coming along quite nicely."

		"Anyways, you must try one of these new potions! I've got a few options, so take your pick!"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Reach for the multi-hued colorful vial of ooze.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"Amazing!" the Brightspark hands you the colorful vial and points toward your mouth. You uncap the vial and quickly drink it. It tickles your throat as the Brightspark suddenly turns red. "You were supposed to rub it on your lips, not drink it!"

			No wonder your throat won't sop itching. "Well," the Brightspark clears his throat, "Let's just hope you find plenty of bathroom stops along the way!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithDamageEventReward(2)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Reach for the dull gray vial of ooze.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"Here you go, handle it with care!" the Brightspark carefully hands you the dull gray vial of ooze. "Shake it and then uncap it when you're ready for a bedazzling chemical reaction!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					await AbilityCmd.InfuseWildElement(null);
					await AbilityCmd.InfuseWildElement(null);
				},
				color =>
					$"At the start of the scenario, {Icons.Inline(Icons.WildElement, color: color)}, {Icons.Inline(Icons.WildElement, color: color)}"
			)
		];
	}
}