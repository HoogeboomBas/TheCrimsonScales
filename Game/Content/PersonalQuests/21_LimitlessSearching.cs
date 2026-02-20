using Fractural.Tasks;

public class LimitlessSearching : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Limitless Searching";
	public override ClassModel ClassToUnlock => ModelDB.Class<StarslingerModel>();
	public override int MaxProgress => 30;
	protected override int AtlasIndex => 21;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.CoinLootedEvent.Subscribe(character, this,
			parameters => parameters.LootObtainer == character,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}