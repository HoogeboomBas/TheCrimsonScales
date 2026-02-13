using System.Collections.Generic;
using Fractural.Tasks;

public class Road20 : RoadEventModel<Road20.ChoiceA, Road20.ChoiceB>
{
	public override int Number => 20;

	public override string Text =>
		"""
		You come across a group of Brightsparks on the side of the road. Dressed in full lab gear, they beckon you toward them and offer you two vials to choose from. One of the vials is filled with a bubbling red liquid and the other with a glowing green ooze.

		"We're running an experiment and would appreciate if you would test one of these out for us," the Brightspark grins. "Just let us know how it goes when you make your way back to the city."
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Try the bubbling red liquid.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You sip the bubbling red liquid, and it has an oddly sour berry flavor. Moments after you finish the last drop, you feel a surge of adrenaline run through your veins. You feel energized and your senses are heightened.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						ScenarioEvents.InitiativesSortedEvent.Subscribe(character, this,
							parameters => true,
							async parameters =>
							{
								ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(character, this,
									initiativeCheckParameters => initiativeCheckParameters.Figure == character,
									initiativeCheckParameters =>
									{
										initiativeCheckParameters.AdjustInitiative(-20);
									}
								);

								await GDTask.CompletedTask;
							},
							EffectType.Selectable,
							effectButtonParameters: new TextEffectButton.Parameters($"{Icons.Inline(character.ClassModel.IconPath)}-20"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Decrease the Initiative of {character.SavedCharacter.GetNameAndIcon()} by 20.")
						);
					}

					ScenarioEvents.RoundEndedEvent.Subscribe(this,
						parameters => true,
						async parameters =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

							foreach(Character character in GameController.Instance.CharacterManager.Characters)
							{
								ScenarioEvents.InitiativesSortedEvent.Unsubscribe(character, this);
								ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(character, this);
							}

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					"At the start of the first round after all cards have been revealed, each character may decrease their leading Initiative by 20."
			)
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Try the glowing green ooze.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You quickly drink the glowing green ooze, and it has a sweet herbaceous taste. Moments after finishing the last drop, you begin to feel groggy and dreary. You turn away sluggishly and your senses feel repressed.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						ScenarioEvents.InitiativesSortedEvent.Subscribe(character, this,
							parameters => true,
							async parameters =>
							{
								ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(character, this,
									initiativeCheckParameters => initiativeCheckParameters.Figure == character,
									initiativeCheckParameters =>
									{
										initiativeCheckParameters.AdjustInitiative(+20);
									}
								);

								await GDTask.CompletedTask;
							}
						);
					}

					ScenarioEvents.RoundEndedEvent.Subscribe(this,
						parameters => true,
						async parameters =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

							foreach(Character character in GameController.Instance.CharacterManager.Characters)
							{
								ScenarioEvents.InitiativesSortedEvent.Unsubscribe(character, this);
								ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(character, this);
							}

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					"At the start of the first round after all cards have been revealed, each character increases their leading Initiative by 20."
			)
		];
	}
}