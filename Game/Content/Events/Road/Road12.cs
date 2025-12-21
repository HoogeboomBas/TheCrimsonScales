using System.Collections.Generic;

public class Road12 : CityEventModel<Road12.ChoiceA, Road12.ChoiceB>
{
	public override int Number => 12;

	public override string Text =>
		"""
		You happen upon a large tent pitched on the side of the road, with a small wooden sign posted on the grass nearby that reads 'FORTUNE TELLER: 5 GOLD.'

		You enter the tent and find an Aesther wrapped in gold robes sitting on a red velvet chair with a glass orb in her hands. She looks up to you with narrow eyes and asks in a soft voice, "Have you come to hear your fortune?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Scoff at the Aesther, you don't believe in fortune telling.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You scoff at the Aesther and state your disbelief in the practice of fortune telling. She narrows her eyes and begins waving her hands over the crystal ball. "Good fortune awaits!" she calls out as you proceed to exit her tent without payment.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			//TODO: All players ignore negative scenario effects
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Pay the Aesther to hear your fortune told.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You pay the Aesther her fee and she begins to wave her hands over the crystal ball while humming a quiet tune. "The battlefield lies ahead. The choices you make will dictate your fortune. Choose wisely and you shall surely prosper."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new LoseCollectiveGoldEventReward(5),
			new GainXPEventReward(3),
			//TODO: All players may select 3 battle goals to choose from instead of 2
		];
	}
}