using System.Collections.Generic;
using Fractural.Tasks;

public class Road32 : CityEventModel<Road32.ChoiceA, Road32.ChoiceB>
{
	public override int Number => 32;

	public override string Text =>
		"""
		Your stomach grumbles loudly as you climb your way through a mountainous area. Wildlife in this area is scarce and rations are low.

		You happen upon a pack of Vermlings nestled in some bushes on the side of a cliff. They're sleeping next to a bag filled with food, and you decide to sneak some from their bag.

		As you approach the bag of food, a light from behind shines in your direction. You turn around to find a Vermling, wide-awake and holding a portable tech lamp. He asks you what you are doing here and you have to react quickly.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Explain you're here for the food and aren't leaving without it.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You explain you're not leaving without food and the Vermling insists he won't share. You grab the bag and begin to run in the opposite direction, and the Vermling gives chase.

			You eventually outrun the Vermling and enjoy a hearty meal, but as you reach your destination you turn around and find the Vermling has caught up and is angrily charging toward you.
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
						}, mandatory: true, hintText: "Select a hex to spawn the elite Vermling Scout"
					);

					if(hex != null)
					{
						Monster monster = await AbilityCmd.SpawnMonster(ModelDB.Monster<VermlingScout>(), MonsterType.Elite, hex);
						if(monster != null)
						{
							monster.SetAlignment(Alignment.Other);
							monster.SetEnemies(Alignment.Characters | Alignment.Enemies | Alignment.Other);
						}
					}
				},
				color =>
					"At the start of the next scenario, an elite Vermling Scout will spawn next to any character. It is an enemy to all figures."
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