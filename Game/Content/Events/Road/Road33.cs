using System.Collections.Generic;

public class Road33 : RoadEventModel<Road33.ChoiceA, Road33.ChoiceB>
{
	public override int Number => 33;

	public override string Text =>
		"""
		Walking alongside a brook, you take notice of a female Orchid meditating by the water. As you peer closer, you can't help but notice her shiny armor looks strikingly familiar. You've untied and been mugged by a similar looking Orchid not too long ago, and have since seen 'wanted' posters with her mugshot posted around town. You're not prepared to let her get away a second time.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Verbally demand the Orchid repay you what she stole.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You approach the Orchid and catch her by surprise. Startled, her hands begin to glow but you explain that you're not interested in a fight and only want to reclaim what's been stolen. She proceeds to explain that she spent all the gold, but she can give you the armor she's wearing if you promise not to report her to the authorities.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveItemEventReward(ModelDB.Item<SilhouetteCuirass>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Use physical force to subdue her.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You sneak up to the Orchid and press your blade against her throat from behind. Not wanting to be harmed, she allows you to tie her arms together with rope. She claims to have spent all the gold, so you proceed to return her to town to collect her bounty instead.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(40)
		];
	}
}