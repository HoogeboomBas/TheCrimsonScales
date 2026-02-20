using System.Collections.Generic;

public class City08 : CityEventModel<City08.ChoiceA, City08.ChoiceB>
{
	public override int Number => 08;

	public override string Text =>
		"""
		You're taking an evening stroll through the Boiler District when you happen upon a group of Vermlings attempting to unscrew a lightbulb from a newly furbished tech lamp.

		They seem to be having trouble unscrewing it with their paws as they recoil from the heat of the bulb.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Point and laugh at the Vermlings for their ineptitude.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You point and laugh at the Vermlings and soon a crowd gathers and joins you in laughter. Embarrassed, the group of Vermlings disband but not before they give you a hard staredown.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AddRoadEventEventReward(ModelDB.Event<Road32>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Their trouble is no laughing matter; give them the help they need.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You help the Vermlings unscrew the lightbulb and they squeel with excitement as it twists out from the socket. They take turns holding it in awe and passing it around as they thank you for your help before scurrying off, leaving the street in darkness.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AddRoadEventEventReward(ModelDB.Event<Road31>())
		];
	}
}