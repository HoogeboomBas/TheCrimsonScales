using System.Collections.Generic;

public class City04 : CityEventModel<City04.ChoiceA, City04.ChoiceB>
{
	public override int Number => 04;

	public override string Text =>
		"""
		As you browse the wares in the Sinking Market, you find two Savvas engaged in a heated argument. As you walk by, they beckon you over to help settle their dispute.

		"This Aeromancer thinks the power of wind is stronger than the power of fire," one of the Savvas remarks.

		"Of course it is!" the other Savvas scoffs in response. "Stranger, which do you deem to be the most powerful? Prove this Rimehearth wrong!"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Side with the Aeromancer and pronounce the power of wind.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You declare the Aeromancer to be correct and the Rimehearth storms off in anger. "Thank you, dear stranger. Here, take this wand so you can show the world how powerful it is to blast your enemies away with a gust of wind."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemReward(ModelDB.Item<WandOfStorms>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Side with the Rimehearth and pronounce the power of fire.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You declare the Rimehearth to be correct and the Aeromancer expresses disgust before turning its head and walking away. "Thank you for supporting the power of flames," the Rimehearth nods. "Here, take this so you can spread the power of fire across the world."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemReward(ModelDB.Item<WandOfInfernos>())
		];
	}
}