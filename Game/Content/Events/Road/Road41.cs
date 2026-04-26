using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road41 : RoadEventModel<Road41.ChoiceA, Road41.ChoiceB>
{
	public override int Number => 41;

	public override string Text =>
		"""
		You happen upon an old beggar on the side of the road. His clothes are tattered and torn, his long white beard is scraggly and he flashes you a smile full of rotting, cracked teeth.

		"Traveler, care to spare a little food?" he croaks in a hoarse voice.

		You brought extra rations and hand a small loaf of bread. The beggar grabs hold of the bread and closes his eyes as he breathes deeply, soaking in its aroma. "Now I bless you in return!" he exclaims. "Care to be blessed with longevity or sustenance?"
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"At the end of the first round, each character may {Icons.Inline(Icons.RecoverCard, textParameters)} one discarded card.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				parameters => parameters.RoundNumber == 1,
				async parameters =>
				{
					ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						AbilityCard selectedAbilityCard =
							await AbilityCmd.SelectAbilityCard(character, CardState.Discarded, mandatory: true,
								hintText: $"Select a discarded card to recover");

						if(selectedAbilityCard != null)
						{
							await AbilityCmd.ReturnToHand(selectedAbilityCard);
						}
					}
				}
			);
		}
	}

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Ask to be blessed with longevity.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You asked to be blessed with longevity. The old man puts his hands over your head and mumbles, leaving a smear of dirt on your forehead. He kisses your cheek and then sits down and begins picking at the bread and licking the crumbs from his hand.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"At the start of the scenario, place one money token in an unoccupied hex adjacent to each character.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				Hex hex = await AbilityCmd.SelectHex(character,
					list => list.AddRange(RangeHelper.GetHexesInRange(character.Hex, 1, false).Where(hex => hex.IsUnoccupied())),
					hintText: "Select an unoccupied hex to spawn a money token in.");

				if(hex != null)
				{
					await AbilityCmd.SpawnCoin(hex);
				}
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Ask to be blessed with sustenance.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You asked to be blessed with sustenance. The old man waves his hands over your head, dropping small bits of dirt in your eyes as he mumbles. He kisses your forehead and then sits down and waves you off.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBOnScenarioStartedReward()
		];
	}
}