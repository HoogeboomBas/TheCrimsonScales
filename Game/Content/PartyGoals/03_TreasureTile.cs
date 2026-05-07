using Fractural.Tasks;

public class TreasureTilePartyGoal : ScalesWithCharactersPartyGoalModel, IEventSubscriber
{
	public override int MaxProgress => 1;

	public override string GetText(int characterCount) => $"All party members loot 1 Treasure Tile in a Scenario each";

	protected override async GDTask OnScenarioSetupPhaseCompleted(ScalesWithCharactersPartyGoalData partyGoalData)
	{
		await base.OnScenarioSetupPhaseCompleted(partyGoalData);

		ScenarioEvents.LootableObjectLootedEvent.Subscribe(this,
			parameters =>
				parameters.LootableObject is Treasure &&
				parameters.LootObtainer is Character,
			async parameters =>
			{
				partyGoalData.AdjustProgress(1, ((Character)parameters.LootObtainer).SavedCharacter);

				await GDTask.CompletedTask;
			}
		);
	}
}