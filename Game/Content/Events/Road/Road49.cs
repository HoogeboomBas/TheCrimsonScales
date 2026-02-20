using System.Collections.Generic;
using Fractural.Tasks;

public class Road49 : RoadEventModel<Road49.ChoiceA, Road49.ChoiceB>
{
	public override int Number => 49;

	public override string Text =>
		"""
		It's a particularly starry night and you happen upon an Aesther looking up to the stars. He ushers you toward him and points up to the sky.

		"See those constellations? I can read them for you. Tell me, what do you see in the stars?"

		You look up to the sky and see a multitude of stars. It's hard to make out a shape, but it looks like there might be stars connecting in the letter 'T' or 'V', but you're unsure which.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Tell the Aesther you see a large 'T' shape in the sky.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Aesther you see the letter 'T' formed in the stars, he shakes his head and tells you, "Tsk, that's the constellation of terror. I see your enemies will be moving forward, even as we speak. Best hurry on, dear travelers, before they advance too quickly."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure is Monster monster)
						{
							ActionState actionState = new ActionState(monster, [MoveAbility.Builder().WithDistance(1).Build()]);
							await actionState.Perform();
						}
					}
				},
				color =>
					$"At the start of the next scenario, all monsters perform a “{Icons.Inline(Icons.Move, color: color)}1” ability."
			)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Tell the Aesther you see a large 'V' shape in the sky.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Aesther you see the letter 'V' formed in the stars. He nods his head in affirmation and tells you, "That's the constellation of victory. You will have an advantage on the battlefield and move as quickly as a shooting star! I bid you well, dear travelers, and success in your journey."
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						ActionState actionState = new ActionState(character, [MoveAbility.Builder().WithDistance(2).Build()]);
						await actionState.Perform();
					}
				},
				color =>
					$"At the start of the next scenario, all characters may perform a “{Icons.Inline(Icons.Move, color: color)}2” ability."
			)
		];
	}
}