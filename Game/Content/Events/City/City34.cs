using System.Collections.Generic;

public class City34 : CityEventModel<City34.ChoiceA, City34.ChoiceB>
{
	public override int Number => 34;

	public override string Text =>
		"""
		You awaken at daybreak in a cold sweat from a terrible nightmare. Disturbed, you decide to pay a visit to Nerro, an Aesther in the Ward of Scales who claims to be an expert in interpreting dreams. After all, you've helped him out in the past and you believe he owes you a favor.

		"Before I begin," Nerro says as he clears his throat, "I must demand payment upfront. Ten gold, please.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Insist he interpret the dream for free in exchange for the favor he owes.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You insist he interpret the dream at no charge and remind him of the time you rescued him from a bera. With an angry look on his face, he consents not to charge you. He pays little attention as you speak, and as you conclude his sour expression turns into a devilish smile as he proceeds to explain that your nightmare was meant to give you a glimpse of the impending misfortune you are due to experience.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Curse)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Pay the asking price of ten gold.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You pay the Aesther what you have and proceed to relay over your dream. He concentrates intensely as he listens, and when you finish speaking he opens his eyes and proceeds to explain that your nightmare was meant to give you a glimpse of the impending misfortune your enemies are due to experience.
			""";

		//TODO: Not enough gold
		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new LoseCollectiveGoldEventReward(10),
			new OnScenarioStartedEventReward(
				async () =>
				{
					await AbilityCmd.CurseMonsters();
					await AbilityCmd.CurseMonsters();
				},
				color =>
					$"Monsters start the scenario with {Icons.Inline(Icons.GetCondition(Conditions.Curse), color: color)}, {Icons.Inline(Icons.GetCondition(Conditions.Curse), color: color)}."
			)
		];
	}
}