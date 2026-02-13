using System.Collections.Generic;
using Fractural.Tasks;

public class Road48 : RoadEventModel<Road48.ChoiceA, Road48.ChoiceB>
{
	public override int Number => 48;

	public override string Text =>
		"""
		Traveling through a dense forest at night, you see a faint green light emanating from a bush off the path. There is a faint whining emanating from the small shrub, and as you turn to walk away, the light intensifies and you hear a whispering voice from behind the bush. "Here here, traveler."

		Interested by the prospect of what may be beckoning you, you approach the bush and see a Vermling hunched behind, waving her hands around. "There is a great spiritual energy here," she explains. "I can feel it. I can impart this upon you, but I warn you - the energy works differently with everyone. Should I restrain the energy and leave you with all but a taste, or are you ready to experience its full potential?"
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Ask her to restrain the energy; you're not quite sure what you can handle.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask the Vermling to restrain the energy and she nods and begins to chant. The light suddenly escapes from the bush and wraps around your body before slowly dimming. It's as if your body absorbed the light, and you begin to feel a strange sensation.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					Figure selectedCharacter = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.Characters[0],
						list => list.AddRange(GameController.Instance.CharacterManager.Characters),
						mandatory: true,
						hintText: () => $"Select a character to gain {Icons.HintText(Icons.GetCondition(Conditions.Muddle))}");

					await AbilityCmd.AddCondition(null, selectedCharacter, Conditions.Muddle);

					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						if(character != selectedCharacter)
						{
							await AbilityCmd.AddCondition(null, character, Conditions.Strengthen);
						}
					}
				},
				color =>
					$"One character starts the scenario with {Icons.Inline(Icons.GetCondition(Conditions.Muddle), color: color)}, all other characters start with {Icons.Inline(Icons.GetCondition(Conditions.Strengthen), color: color)}."
			)
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Tell her to hold nothing back. You're ready for anything.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask the Vermling to hold nothing back and she begins to shriek and wave her hands frantically in the air. The light intensifies until it becomes blinding and you shudder as you close your eyes. All of a sudden, her shrieking stops and you open your eyes to find nothing but a singed twig in the place of the bush, and a burning sensation running through you like nothing you've ever experienced before. 
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					Figure selectedCharacter = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.Characters[0],
						list => list.AddRange(GameController.Instance.CharacterManager.Characters),
						mandatory: true,
						hintText: () => $"Select a character to gain {Icons.HintText(Icons.GetCondition(Conditions.Stun))}");

					await AbilityCmd.AddCondition(null, selectedCharacter, Conditions.Stun);

					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						if(character != selectedCharacter)
						{
							await AbilityCmd.AddCondition(null, character, Conditions.Invisible);
						}
					}
				},
				color =>
					$"One character starts the scenario with {Icons.Inline(Icons.GetCondition(Conditions.Stun), color: color)}, all other characters start with {Icons.Inline(Icons.GetCondition(Conditions.Invisible), color: color)}."
			)
		];
	}
}