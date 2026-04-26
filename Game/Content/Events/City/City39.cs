using System.Collections.Generic;

public class City39 : CityEventModel<City39.ChoiceA, City39.ChoiceB>
{
	public override int Number => 39;

	public override string Text =>
		"""
		"Help repair the wall!" a young boy waves a sign toward people walking by the pier. "Paying top wages to anyone who can help repair the wall!"

		Curious, you approach the boy and inquire about his offering. He explains that the wall was recently damaged due to an Inox invasion and the city is offering top coin to anyone willing to aid in the repairs.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Accept the job. After all, you could use the extra coin.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You accept the job and spend the day hauling stone for the builders. Weary from a full day's work, you accept the payment and head to the Sleeping Lion to enjoy the rest of the night off.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainGoldEachReward(10)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Volunteer your time for free for a worthy cause.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You offer to volunteer and the boy points you in the direction of the wall. You spend hours helping out where you can, and at the end of the day the repairmen offer to share of their payment in appreciation for your hard work.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainGoldEachReward(5),
			new GainReputationReward(1)
		];
	}
}