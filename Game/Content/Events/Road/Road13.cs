using System.Collections.Generic;
using Fractural.Tasks;

public class Road13 : CityEventModel<Road13.ChoiceA, Road13.ChoiceB>
{
	public override int Number => 13;

	public override string Text =>
		"""
		You happen upon a large tent pitched on the side of the road, with a small wooden sign posted on the grass nearby that reads 'FORTUNE TELLER: 5 GOLD.'

		You enter the tent and find an Aesther wrapped in gold robes sitting on a blue velvet chair with a glass orb in her hands. She looks up to you with narrow eyes and asks in a soft voice, "Have you come to hear your fortune?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Scoff at the Aesther, you don't believe in fortune telling.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You scoff at the Aesther and state your disbelief in the practice of fortune telling. She narrows her eyes and begins waving her hands over the crystal ball. "Bad fortune awaits!" she calls out as you proceed to exit her tent without payment.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						character.AMDCardDeck.AddMinusOne();
						character.AMDCardDeck.AddMinusOne();
					}

					await GDTask.CompletedTask;
				},
				color => $"All characters start the next scenario with two extra “-1” AMD cards."
			)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Pay the Aesther to hear your fortune told.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You pay the Aesther her fee and she begins to wave her hands over the crystal ball while humming a quiet tune. "Good fortune awaits! Your enemies will be crippled and you will have the upper hand in battle."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new LoseCollectiveGoldEventReward(5),
			new OnScenarioStartedEventReward(
				async () =>
				{
					GameController.Instance.MonsterAMDCardDeck.AddMinusOne();
					GameController.Instance.MonsterAMDCardDeck.AddMinusOne();

					await GDTask.CompletedTask;
				},
				color => $"Monsters start the next scenario with two extra “-1” AMD cards."
			)
		];
	}
}