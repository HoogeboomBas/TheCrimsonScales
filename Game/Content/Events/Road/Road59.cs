using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road59 : RoadEventModel<Road59.ChoiceA, Road59.ChoiceB>
{
	public override int Number => 59;

	public override string Text =>
		"""
		"Red berries, green berries, get your berries here!" you hear a melodious choir chanting froma  caravan slowly passing you by. A Vermling pokes it head out of the caravan, and it comes to a screeching halt. "Traveler! Have you ever seen or tasted such a spectacular delicacy?" the Vermling extends both hands to reveal a handful of sticky berries, juice dripping from his paws.

		You shake your head in disgust, remembering your promise to never eat roadside berries again, when the Vermling shakes his head and grabs your shoulder. "These are no ordinary berries. They're infused with the psychic power which will allow you to manifest your greatest inner potential. Therefore, I insist! Take your pick, red or green?"
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Choose the red berries.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Naturalists or StartingGroup.Trailblazers;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state) =>
			"""
			You choose to eat the red berries, and although the taste is familiar, you begin to sense the power described by the vermling.
			""";

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new OnScenarioStartedEventReward(
						async () =>
						{
							ScenarioEvents.RoundEndedEvent.Subscribe(this,
								parameters => parameters.RoundNumber == 1,
								async parameters =>
								{
									ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

									foreach(Character character in GameController.Instance.CharacterManager.Characters)
									{
										AbilityCard selectedAbilityCard =
											await AbilityCmd.SelectAbilityCard(character, CardState.Lost, mandatory: true,
												hintText: $"Select a lost card to recover");

										if(selectedAbilityCard != null)
										{
											await AbilityCmd.ReturnToHand(selectedAbilityCard);
										}
									}
								}
							);

							await GDTask.CompletedTask;
						},
						color =>
							$"At the end of the first round, each character may {Icons.Inline(Icons.RecoverCard, color: color)} one lost card."
					)
				];
			}
			else
			{
				return
				[
					new OnScenarioStartedEventReward(
						async () =>
						{
							foreach(Character character in GameController.Instance.CharacterManager.Characters)
							{
								character.SetHealth(character.MaxHealth + 3);

								await GDTask.CompletedTask;
							}
						},
						color =>
							$"All characters start the next scenario with 3 more hit points."
					)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Choose the green berries.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Trailblazers or StartingGroup.Explorers;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state) =>
			"""
			You choose to eat the green berries, and although the taste is familiar, you begin to sense the power described by the vermling.
			""";

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new OnScenarioStartedEventReward(
						async () =>
						{
							ScenarioEvents.AbilityStartedEvent.Subscribe(this,
								parameters =>
									parameters.Performer is Character &&
									parameters.AbilityState is MoveAbility.State &&
									GameController.Instance.ScenarioPhaseManager.RoundIndex == 0,
								async parameters =>
								{
									MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
									moveAbilityState.AdjustMoveValue(moveAbilityState.MoveValue);

									await GDTask.CompletedTask;
								}
							);

							ScenarioEvents.RoundEndedEvent.Subscribe(this,
								parameters => parameters.RoundNumber == 1,
								async parameters =>
								{
									ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
									ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

									await GDTask.CompletedTask;
								}
							);

							await GDTask.CompletedTask;
						},
						color =>
							$"During the first round, all move abilities performed by characters have their value doubled."
					)
				];
			}
			else
			{
				return
				[
					new OnScenarioStartedEventReward(
						async () =>
						{
							ScenarioEvents.RoundEndedEvent.Subscribe(this,
								parameters => parameters.RoundNumber == 1,
								async parameters =>
								{
									ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

									foreach(Character character in GameController.Instance.CharacterManager.Characters)
									{
										ItemModel selectedItem = await AbilityCmd.SelectItem(character,
											character.Items.Where(item => item.ItemState is ItemState.Spent or ItemState.Consumed).ToList(),
											hintText: "Select an item to refresh");

										if(selectedItem != null)
										{
											await AbilityCmd.RefreshItem(selectedItem);
										}
									}
								}
							);

							await GDTask.CompletedTask;
						},
						color =>
							$"At the end of the first round, each character may {Icons.Inline(Icons.RecoverCard, color: color)} one spent or consumed item."
					)
				];
			}
		}
	}
}