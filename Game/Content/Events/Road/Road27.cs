using System.Collections.Generic;

public class Road27 : CityEventModel<Road27.ChoiceA, Road27.ChoiceB>
{
	public override int Number => 27;

	public override string Text =>
		"""
		You're passing by a caravan when you hear a muffled scream from within. You peer in out of curiosity and see an Orchid tied up to a chair in an otherwise empty caravan.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Untie the ropes and free the captive Orchid.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin to untie the ropes from around the Orchid. The Orchid stands up, revealing shining silhouette armor. All of a sudden, the Orchid's hands begin to glow and a devious smile grows across her face. Before you can react, she blasts you out of the caravan with a powerful spell and dashes out onto the road.

			As you dust off your clothes, you reach into your pockets and realize you've been pickpocketed.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AddRoadEventEventReward(ModelDB.Event<Road33>()),
			new LoseCollectiveGoldEventReward(10)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Wait outside for the captors to return. There must be a good reason for this.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			After patiently waiting for about an hour, several large Inox wearing heavy armor wrapped in chains approach the caravan. You question the circumstances and the Inox explain that they're bounty hunters, and this Orchid is a dangerous wanted criminal. They thank you with a gift for guarding the caravan and begin to prepare for the next leg of their journey.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<StaffOfRetribution>())
		];
	}
}