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

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"All characters start the next scenario with 2 more hit points.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				character.SetHealth(character.MaxHealth + 2);
			}
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Tell the Aesther you're in pursuit of health.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Aesther you're in pursuit of health and he points to the stars. "I'll be sure to wish upon a certain star for you tonight, and should the constellations align, you will find yourself in full health!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"The first character to loot a treasure tile this scenario gains an additional {Icons.Inline(Icons.Coins, textParameters)}10.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			ScenarioEvents.LootableObjectLootedEvent.Subscribe(this,
				parameters =>
					parameters.LootableObject is Treasure &&
					parameters.LootObtainer is Character,
				async parameters =>
				{
					await AbilityCmd.GainGold((Character)parameters.LootObtainer, 10);

					ScenarioEvents.LootableObjectLootedEvent.Unsubscribe(this);

					await GDTask.CompletedTask;
				}
			);
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Tell the Aesther you're in pursuit of wealth.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Aesther you're in pursuit of wealth and he points to the stars. "I'll be sure to wish upon a certain star for you tonight, and should the constellations align, you will find yourself with great fortune!"
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBOnScenarioStartedReward()
		];
	}
}