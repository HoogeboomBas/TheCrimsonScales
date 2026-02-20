using System.Collections.Generic;
using Fractural.Tasks;

public class Road50 : RoadEventModel<Road50.ChoiceA, Road50.ChoiceB>
{
	public override int Number => 50;

	public override string Text =>
		"""
		Walking along a brook late at night, you come across a familiar Aesther gazing up at the stars. You recognize the Aesther as the Starslinger, and it greets you with a warm smile.

		"Greetings! I will be exploring the galaxies tonight and will be watching you from above. Tell me, will you persue health or wealth tonight?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Tell the Aesther you're in pursuit of health.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Aesther you're in pursuit of health and he points to the stars. "I'll be sure to wish upon a certain star for you tonight, and should the constellations align, you will find yourself in full health!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						character.SetHealth(character.MaxHealth + 2);

						await GDTask.CompletedTask;
					}
				},
				color =>
					$"All characters start the next scenario with 2 more hit points."
			)
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Tell the Aesther you're in pursuit of wealth.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Aesther you're in pursuit of wealth and he points to the stars. "I'll be sure to wish upon a certain star for you tonight, and should the constellations align, you will find yourself with great fortune!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					ScenarioEvents.LootableObjectLootedEvent.Subscribe(this,
						parameters =>
							parameters.LootableObject is Treasure &&
							parameters.LootObtainer is Character,
						async parameters =>
						{
							ScenarioEvents.LootableObjectLootedEvent.Unsubscribe(this);

							GameController.Instance.EndEvent += (result, progress) =>
							{
								Character character = (Character)parameters.LootObtainer;
								character.SavedCharacter.AddGold(10);
							};

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					$"The first character to loot a treasure tile this scenario gains an additional {Icons.Inline(Icons.Coins, color: color)}10."
			)
		];
	}
}