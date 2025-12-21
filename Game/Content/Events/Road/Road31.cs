using System.Collections.Generic;
using Fractural.Tasks;

public class Road31 : CityEventModel<Road31.ChoiceA, Road31.ChoiceB>
{
	public override int Number => 31;

	public override string Text =>
		"""
		Your stomach grumbles loudly as you climb your way through a mountainous area. Wildlife in this area is scarce and rations are low.

		You happen upon a pack of Vermlings nestled in some bushes on the side of a cliff. They're sleeping next to a bag filled with food, and you decide to sneak some from their bag.

		As you approach the bag of food, a light from behind shines in your direction. You turn around to find a Vermling, wide-awake and holding a portable tech lamp. He asks you what you are doing here and you have to react quickly.
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Explain you're here for the food and aren't leaving without it.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You explain you're not leaving without food and the Vermling insists he won't share. As you continue to argue with the Vermling he shines the light toward your face and stares for a few seconds, before smiling cheerfully and claiming to recognize you. He mentions something about you helping him unscrew a lightbulb from a tech lamp and sends you away feeling invigorated with a full stomach.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					foreach(Character character in GameController.Instance.CharacterManager.Characters)
					{
						ScenarioEvents.RoundStartedBeforeInitiativesSortedEvent.Subscribe(character, this,
							parameters => true,
							async parameters =>
							{
								ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(character, this,
									initiativeCheckParameters => initiativeCheckParameters.Figure == character,
									initiativeCheckParameters =>
									{
										initiativeCheckParameters.SetInitiative(1);
									}
								);

								await GDTask.CompletedTask;
							},
							EffectType.Selectable,
							effectButtonParameters: new TextEffectButton.Parameters($"{Icons.Inline(character.ClassModel.IconPath)}01"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Set the Initiative of {character.SavedCharacter.GetNameAndIcon()} to 01.")
						);
					}

					ScenarioEvents.RoundEndedEvent.Subscribe(this, parameters => true,
						async parameters =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

							foreach(Character character in GameController.Instance.CharacterManager.Characters)
							{
								ScenarioEvents.RoundStartedBeforeInitiativesSortedEvent.Unsubscribe(character, this);
								ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(character, this);
							}

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					"During the first round of the scenario, before any cards are revealed, any characters may declare to act on Initiative 01 instead of their leading Initiative."
			)
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Tell the Vermling you lost your way.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Vermling you lost your way and he warns you to leave.

			You continue on with your journey but have no luck finding more food. You manage to make it to your destination but feel sluggish and worn out from the lack of sustenance.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(this,
						parameters =>
							parameters.Figure is Character,
						parameters =>
						{
							parameters.SetInitiative(99);
						}
					);

					ScenarioEvents.RoundEndedEvent.Subscribe(this,
						parameters => true,
						async parameters =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
							ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(this);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					"During the first round of the scenario, all characters act on Initiative 99 instead of their leading Initiative."
			)
		];
	}
}