using System.Collections.Generic;
using Fractural.Tasks;

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

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionReward(Conditions.Curse)
		];
	}

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Monsters start the scenario with {Icons.Inline(Icons.GetCondition(Conditions.Curse), textParameters)}, {Icons.Inline(Icons.GetCondition(Conditions.Curse), textParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			await AbilityCmd.CurseMonsters();
			await AbilityCmd.CurseMonsters();
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Pay the asking price of ten gold.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You pay the Aesther what you have and proceed to relay over your dream. He concentrates intensely as he listens, and when you finish speaking he opens his eyes and proceeds to explain that your nightmare was meant to give you a glimpse of the impending misfortune your enemies are due to experience.
			""";

		//TODO: Not enough gold
		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new LoseCollectiveGoldReward(10),
			new ChoiceBOnScenarioStartedReward()
		];
	}
}