using System.Collections.Generic;

public class City45 : CityEventModel<City45.ChoiceA, City45.ChoiceB>
{
	public override int Number => 45;

	public override string Text =>
		"""
		Taking work whenever it comes by, you've accepted a night job to unload a ship by the Old Docks. As you approach the vessel you find an impressively large Lurker standing by, and it's emitting vibrant glowing lights. There are no lamps around, but the colorful glows emanating from the Lurker provide you with enough light to perform the job with ease.

		After the job is done and the shipowner pays you, the Lurker approaches you and blocks your path. It points to the pouch of coin with one of its claws, indicating that it would like some gold for providing you with good light.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Pay the Lurker from your share of profits.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			With plenty of profit to share, you reach into your pouch and hand a few gold coins to the Lurker. It clicks its claws together and happily scurries away, and you head to the Sleeping Lion to enjoy a well-deserved night of relaxation.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldReward(20)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Refuse to give the Lurker any of your hard-earned gold";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You firmly explain to the Lurker that it won't be getting any gold from you tonight. Before you can gauge its reaction, the Lurker swiftly grabs the pouch from your hand with its claw and lobs it far into the ocean. You hear a soft splash in the distance as the Lurker quickly scurries away, leaving you bewildered and empty-handed.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}