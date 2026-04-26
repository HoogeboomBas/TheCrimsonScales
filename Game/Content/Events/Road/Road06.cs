using System.Collections.Generic;

public class Road06 : RoadEventModel<Road06.ChoiceA, Road06.ChoiceB>
{
	public override int Number => 06;

	public override string Text =>
		"""
		Making your way across a narrow river leading over a waterfall, you see a large turtle stuck between two rocks. The rushing current is pulling stones and logs toward it and it's only a matter of time before an object knocks it over the waterfall.

		You feel that something should be done to rescue it, but the current is strong and it's a long way down the waterfall.
		""";

	public class ChoiceASummonReward : SummonReward
	{
		public override SummonAbility SummonAbility { get; } =
			SummonAbility.Builder()
				.WithName("Snapping Turtle")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/giant_tortoise_AI.png") //TODO: Generic or AI summon visual?
				.WithHealth(3)
				.WithMove(1)
				.WithAttack(2)
				.WithTraits(new ShieldTrait(1))
				.Build();
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Jump into the river to save the turtle.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You leap into the river and grab the turtle. You quickly duck into the water as the current pulls a large log over your head, and you barely make it out before an entire tree is swept over the falls. You managed to save the large turtle just in the nick of time.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceASummonReward()
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Try to pull the turtle out with a stick.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You reach for a long stick and use it to try lifting the turtle out of the rocks. You succeed, but the stick breaks and the turtle is sent flying over the waterfall.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}