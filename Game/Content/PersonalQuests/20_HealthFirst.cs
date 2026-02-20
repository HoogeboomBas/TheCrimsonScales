using Fractural.Tasks;

public class HealthFirst : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Health First";
	public override ClassModel ClassToUnlock => ModelDB.Class<StarslingerModel>();
	public override int MaxProgress => 7;
	protected override int AtlasIndex => 20;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.ScenarioEndedEvent.Subscribe(character, this,
			parameters =>
				parameters.Win &&
				!character.IsDamaged(),
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}