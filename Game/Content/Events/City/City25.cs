using System.Collections.Generic;

public class City25 : CityEventModel<City25.ChoiceA, City25.ChoiceB>
{
	public override int Number => 25;

	public override string Text =>
		"""
		You're walking through the Old Docks late at night when you turn into an alleyway and happen upon a group of bandits beating a helpless old man. It's clear that he's being mugged, and there's nobody else in sight.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Help the man by fighting the muggers off.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You draw your blade and charge forward with a roar. Not wanting to be harmed, the bandits scatter in all directions, leaving the grateful old man with his coin behind.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainXPReward(8),
			new GainProsperityReward(1)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Help the bandits by joning them in the fray.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You jump into the fray and begin kicking the man as he lays on the floor groaning with his arms wrapped around his knees. After a minute or so, the man agrees to empty his pockets and the bandits leave you with your share of the plunder.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldReward(15),
			new LoseReputationReward(1)
		];
	}
}