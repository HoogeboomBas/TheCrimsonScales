using System.Collections.Generic;
using Fractural.Tasks;

public class City28 : CityEventModel<City28.ChoiceA, City28.ChoiceB>
{
	public override int Number => 28;

	public override string Text =>
		"""
		You're perusing through the market when a Vermling Shaman approaches you with one trinket in each hand. "Blessed be the one who chooses fortune, cursed be the one who chooses faith," the Shaman rambles. "Blessed be the one who chooses wealth, cursed be the one who chooses prosperity."

		"Which will you choose?" the shaman lifts up the trinkets to reveal two amulets dangling from their chains. "The amulet of security or the amulet of restoration?"
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Choose the amulet of security.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You point to the amulet of security and a smile forms upon the Vermling's face. "You shall be blessed! Blessings upon you!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					ScenarioCheckEvents.MoneyTokenValueCheckEvent.Subscribe(this,
						parameters => true,
						parameters =>
						{
							parameters.AdjustValue(1);
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					$"All money tokens acquired during the next scenario are worth {Icons.Inline(Icons.Coins, color: color)}1 more each."
			)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Choose the amulet of restoration.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You point to the amulet of restoration and a frown falls upon the Vermling's face. "You have been warned! Curses upon you!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Curse)
		];
	}
}