using System.Collections.Generic;

public class City24 : CityEventModel<City24.ChoiceA, City24.ChoiceB>
{
	public override int Number => 00;

	public override string Text =>
		"""
		You've spent the day hauling boxes from one warehouse to another in the Boiler District for a Valrath you met last night at the Sleeping Lion. He offered enough coin to last for many nights of drinking for the job, and with such high pay it was a job you couldn't refuse.

		After placing the last box on the floor and wiping the sweat from your brow, you approach the Valrath outside the warehouse to demand payment. "Here you go, ten gold," he sneers as he drops the coins by your feet. You could make due with the disrespect, but that wasn't the agreed upon sum and you've worked hard all day.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Kindly request the Valrath pay you what he promised.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You explain to the Valrath that the price was 30 gold and you have several witnesses in the Sleeping Lion to corroborate your story. Not wanting to soil his reputation at the tavern, the Valrath grumbles as he hands you the rest of the coin before shooing you away.

			It'll be the last time you work for him, but at least you've earned your keep for today.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(30)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Beat him until he gives you what he owes.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You begin to beat upon the Valrath who quickly drops to the ground in fear. He begs for mercy as you deal each blow and offers to empty his wallet. Having taught the Valrath a lesson, you hastily take all the coin he has to offer before leaving him to softly weep alone on the floor.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(40),
			new LoseReputationEventReward(1)
		];
	}
}