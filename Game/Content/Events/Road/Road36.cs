using System.Collections.Generic;

public class Road36 : RoadEventModel<Road36.ChoiceA, Road36.ChoiceB>
{
	public override int Number => 36;

	public override string Text =>
		"""
		You come across a group of men huddled together on the side of the road. They seem intensely focused on something as they mutter under their breath while exchanging colorful concoctions and pour various smoking liquids into a pot. You vaguely recognize one of the men as a Brightspark but he's too busy concentrating on his work to notice you.

		"Come, travelers! Come see this marvelous experiment!" one of the men gleefully explains as he motions you forward. "You seem like a strong, capable group. We could use some help stirring and mixing the heavy brew."

		You glance over and see a bubbling cauldron with sparks flying all over.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Help the men mix the brew.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			As you approach the group of men, you recognize one particular Brighspark from your previous adventures. This warm sense of familiarity provides you with a sense of trust as you proceed to help the men mix their concoction. After several minutes of tiresome mixing, the brew turns a bright orange color. "We've done it!" the Brightspark jumps for joy as he grabs hold of your shoulders and shakes them with joy. "Here, have some! It will surely help you."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionReward(Conditions.Bless, Conditions.Bless)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Decline and continue forward.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			The men among the group angrily scold you for not wanting to help. Although you declined in a polite manner, the men begin unclipping vials of liquid from their belts and start throwing them in your direction. You flee the scene, but not before enduring a few bruises and burns from the pelting.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithDamageReward(3)
		];
	}
}