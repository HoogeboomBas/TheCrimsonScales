using System.Collections.Generic;

public class City20 : CityEventModel<City20.ChoiceA, City20.ChoiceB>
{
	public override int Number => 20;

	public override string Text =>
		"""
		You've enjoyed a night full of laughs and good drinks in the Sleeping Lion when you begin to prepare to head out for the night. "Wait!" a Quatryl you knw well, Shiela, stumbles in your direction. Clearly she's had one too many drinks. "Where... where do I..." she gasps as she suddenly collapses to the ground.

		The bartender looks to you with stern eyes as if expecting you to take responsibility for this situation. You're familiar with where she lives, but it's a long walk to the Mixed District.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Sling her over your back and carry her to her apartment in the Mixed District.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You lift Shiela up over your shoulder and proceed to carry her all the way to the Mixed District. As you head up the stairs to her apartment, Shiela slurs a hearty thanks and promises to repay you the next time you visit her potion shop.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new DowntimeShopPriceEventReward(
				eventReward =>
					parameters =>
					{
						if(!state.GetCustomValue<bool>(parameters.Buyer.Guid.ToString()) && parameters.ItemModel.ItemType == ItemType.Small)
						{
							parameters.AdjustPrice(-10);
						}
					},
				eventReward =>
					parameters =>
					{
						if(!state.GetCustomValue<bool>(parameters.Buyer.Guid.ToString()) && parameters.ItemModel.ItemType == ItemType.Small)
						{
							state.SetCustomValue(parameters.Buyer.Guid.ToString(), true);
						}
					},
				color =>
					$"During this City Phase, each character may buy one {Icons.Inline(Icons.GetItem(ItemType.Small), color: color)} item from the shop for {Icons.Inline(Icons.Coins, color: color)}10 less.")
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Drag her out into the alleyway and go home to get a good night's rest.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You take hold of Shiela as the bartender calls out to thank you. As soon as you reach the alleyway outside, you look around to ensure there are no witnesses before gently placing her body down next to a heap of garbage, carefully avoiding the broken glass scattered on the ground.

			You head straight home with little regard for her wellbeing and proceed to get a full night's rest.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Strengthen)
		];
	}
}