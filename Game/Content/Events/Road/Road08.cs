using System.Collections.Generic;

public class Road08 : RoadEventModel<Road08.ChoiceA, Road08.ChoiceB>
{
	public override int Number => 08;

	public override string Text =>
		"""
		You're walking through a mountainous area as snow begins to fall from the sky. As you traverse further closer to the mountaintop, you encounter deep snowbanks but continue trudging through when you take notice of a young Vermling huddled up beneath a tree. She's trembling and visibly shaken, and you inquire as to what's wrong.

		"I've never seen anything like this," the Vermling shudders. "White powder falling from the sky, and it's cold! Oh, so cold! I'm afraid of leaving here since I don't know what it will do to me, will you kindly lead me away from whatever this is? I'll gladly pay you good coin!"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Explain to the Vermling that it's harmless snow.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You explain to the Vermling that it's harmless snow, and snowfall is natural this high up in the peaks. She thanks you for clearing up the confusion and offers to accompany you on your journey.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					Hex hex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.GetCharacter(0),
						list =>
						{
							foreach(Character character in GameController.Instance.CharacterManager.Characters)
							{
								foreach(Hex hex in RangeHelper.GetHexesInRange(character.Hex, 1, false))
								{
									if(hex.IsEmpty())
									{
										list.AddIfNew(hex);
									}
								}
							}
						}, hintText: "Select a hex to spawn the allied Vermling Scout"
					);

					if(hex != null)
					{
						Monster monster = await AbilityCmd.SpawnMonster(ModelDB.Monster<VermlingScout>(), MonsterType.Normal, hex);
						if(monster != null)
						{
							monster.SetAlignment(Alignment.Characters);
							monster.SetEnemies(Alignment.Enemies);
						}
					}
				},
				color => "At the start of the next scenario, an allied Vermling Scout will spawn next to any character."
			)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Quietly accept the job and agree to escort the Vermling.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You agree to escort the Vermling out of the mountains and lead her out of the snowy mountains. She thanks you, pays you and vows never to return to what she calls the 'cursed powder mountains' again.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(15)
		];
	}
}