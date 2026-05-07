using System.Collections.Generic;

public class City33 : CityEventModel<City33.ChoiceA, City33.ChoiceB>
{
	public override int Number => 33;

	public override string Text =>
		"""
		"Hey, I remember you!" you hear a gruff voice exclaim from behind you. You weren't expecting to see any familiar faces in your visit to the Sinking Market, but as you turn around you come face-to-face with a bearded man.

		"I was stuck in that cave and you pulled me up and rescued me," the man reaches out and pats your shoulder. "If not for you, I'd still be stuck down there - or who knows what could've eaten me!"

		"Well," the man says as he bends down to tie his boots, which look to be in brand new condition. "Has karma treated you well?" 
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Reply that things have been going well since your last encounter.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the man that things have been going well since your last encounter. "I knew karma would take care of you! Always does!"

			He stands up from tying his shoes and lifts them up for you to see. "Great pair, by the way. Why don't I tell you where I got them from, in case you ever find yourself needing a pair?"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainItemDesignReward(ModelDB.Item<LightweightBoots>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Remind the man that he has yet to repay you for your kindness.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You remind the man that he left you empty-handed that day and his face turns red as he begins to stammer. "Well, I uh... haven't got much..."

			You point toward the shiny pair of boots he's wearing and make it clear that he won't be leaving without repaying your kindness. "What's a pair of shoes anyways" he laughs nervously as he slips them off and hands them to you.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemReward(ModelDB.Item<LightweightBoots>())
		];
	}
}