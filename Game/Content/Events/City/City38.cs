using System.Collections.Generic;

public class City38 : CityEventModel<City38.ChoiceA, City38.ChoiceB>
{
	public override int Number => 38;

	public override string Text =>
		"""
		What started off as an ordinary day in the market turned into a happenstance reunion between you and an old acquaintance.

		"Great to see you again!" the Orchid Chieftain greets you with a smile. "Come, join me in the Sleeping Lion tonight and let's share a few drinks and reminisce."
		""";

	public class ChoiceADowntimeEnhancementCostReward : DowntimeEnhancementCostReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"The next enchantment purchased this City Phase will cost {Icons.Inline(Icons.Coins, textParameters)}30 less.";

		public ChoiceADowntimeEnhancementCostReward()
		{
		}

		protected override void CalculateCostApplyFunction(BetweenScenariosEvents.CalculateEnhancementCost.Parameters parameters)
		{
			parameters.AdjustCost(-30);
		}

		protected override void EnhancementBoughtApplyFunction(BetweenScenariosEvents.EnhancementBought.Parameters parameters)
		{
			Complete();
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Agree to meet the Chieftain in the Sleeping Lion this evening.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You agree to join the Orchid under the condition that she sponsor the drinks. After a night of laughter and reminiscing, the Orchid offers you a token of appreciation for the great evening.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceADowntimeEnhancementCostReward()
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Politely decline, you have better things to do tonight.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			The chieftain is saddened by your declination and vows to have a good time without you. To assuage her disappointment, you offer to sponsor drinks for the evening.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new LoseCollectiveGoldReward(10)
		];
	}
}