using System.Collections.Generic;

public class City40 : CityEventModel<City40.ChoiceA, City40.ChoiceB>
{
	public override int Number => 40;

	public override string Text =>
		"""
		Having enjoyed a hearty meal at your favorite restaurant, the Salty Duck, you prepare to pay the bill when a squadron of Quatryl Bombards enter the establishment and are seated at a table next to yours. You can't help but overhear their conversation about an upcoming shipment of military equipment arriving at the docks later tonight.

		They discuss the details of the inbound shipment and vent their frustration for not having found any temporary hires to help supervise the shipment, which is currently arriving unattended.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Approach the Bombards and offer to supervise the shipment - for good coin.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You approach the Bombards and offer to supervise the shipment for a fee. Happy to have found someone to watch the shipment, they offer you a hefty sum but warn you that they'll be taking inventory afterwards and will hunt you down if anything goes missing.

			The night proves uneventful as you proceed to supervise the shipment. Ensuring all inventory remains intact. A Bombard greets you later that night at the Sleeping Lion and pays you in full for a job well done.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldReward(20)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Plan to sneak there later that night to steal some equipment for yourself.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You take note of the details of the shipment and arrive at the docks late at night, finding several crates of unattended goods. You help yourself to a piece of equipment from within the crate and quickly exit the scene before a witness could show up.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemReward(ModelDB.Item<CanisterProjectile>())
		];
	}
}