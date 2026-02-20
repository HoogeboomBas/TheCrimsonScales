using System.Collections.Generic;

public class City44 : CityEventModel<City44.ChoiceA, City44.ChoiceB>
{
	public override int Number => 44;

	public override string Text =>
		"""
		"STOP, THIEF!"

		The roar from the crowd in the bustling market startles you as you take notice of a young hooded girl carrying a large purse running through the crowd. She intentionally knocks barrels of fruit into the path behind her as she shoves bystanders out of her way, heading in your direction.

		An impressively large Inox struggles to catch his breath as he chases after her, swinging a large metal chain in the air like a lasso. Nobody else seems to be making an attempt to stop the supposed thief.

		As she makes her way closer to you, now is the chance to stop her.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Tackle the alleged thief.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			As the girl approaches, you quickly leap toward her and tackle her to the ground. The inox swings his chain forward and shackles her feet together. After catching his breath, he begins to thank you. "She's been wanted in several districts for thievery," the Inox explains as he pulls out a large pouch. "Thank you for your efforts. I'll be collecting a great bounty for this one. Here's some coin for your help."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainGoldEachEventReward(15),
			new GainReputationEventReward(1)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Move out of the way, this isn't your chase.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You attempt to step out of the way but the girl shoves you aside, causing you to stumble into a nearby stall containing glassware. You hear a large crash as you fall to the ground, and the shop-owner angrily demands compensation for the broken goods.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new LoseCollectiveGoldEventReward(15)
		];
	}
}