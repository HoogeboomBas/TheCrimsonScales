using System.Collections.Generic;
using Fractural.Tasks;

public class Road42 : RoadEventModel<Road42.ChoiceA, Road42.ChoiceB>
{
	public override int Number => 42;

	public override string Text =>
		"""
		As you make your way down a cobblestone road, you find a man with a long white flowing beard kneeling down on the side of the road, mumbling to himself. As you pass him by, he looks up to you and clears his throat.

		"Travelers, I am praying on behalf of all who merit to cross my path. Tell me, are you on the path of righteousness or the path of despair?"
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Respond with \"path of righteousness\".";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You answer that you are on the path of righteousness and the man smiles warmly. "Very well then. I shall pray for you to continue to walk the path of prosperity and good merit!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					await AbilityCmd.InfuseElement(null, Element.Light, immediately: true);
				},
				color => $"At the start of the next scenario, {Icons.Inline(Icons.GetElement(Element.Light), color: color)}."
			),
			new OnScenarioStartedEventReward(
				async () =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(this,
						parameters =>
							parameters.Performer is Character &&
							parameters.Type == AMDCardType.Value &&
							parameters.Value < 0,
						async parameters =>
						{
							parameters.SetValue(+1);

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.RoundEndedEvent.Subscribe(this,
						parameters => true,
						async paramers =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
							ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(this);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					$"During the first round of the next scenario, any negative attack modifier cards drawn by players to be a +1 instead."
			)
		];
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Respond with \"path of despair\".";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You answer that you are on the path of despair and the man instantly frowns. "Well, if that's the path you choose, I shall pray for you to continue to walk the path of anguish and dishearten your enemies!"
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					await AbilityCmd.InfuseElement(null, Element.Dark, immediately: true);
				},
				color => $"At the start of the next scenario, {Icons.Inline(Icons.GetElement(Element.Dark), color: color)}."
			),
			new OnScenarioStartedEventReward(
				async () =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(this,
						parameters =>
							parameters.Performer is Character &&
							parameters.Type == AMDCardType.Value &&
							parameters.Value < 0,
						async parameters =>
						{
							parameters.SetValue(-1);

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.RoundEndedEvent.Subscribe(this,
						parameters => true,
						async paramers =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
							ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(this);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				color =>
					$"During the first round of the next scenario, any negative attack modifier cards drawn by players to be a -1 instead."
			)
		];
	}
}