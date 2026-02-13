using Fractural.Tasks;

public class ExperiencedLeader : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Experienced Leader";
	public override ClassModel ClassToUnlock => ModelDB.Class<ChieftainModel>();
	public override int MaxProgress => 12;
	protected override int AtlasIndex => 7;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.ScenarioEndedEvent.Subscribe(character, this,
			parameters =>
				parameters.Win &&
				character.ObtainedXP >= 12,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}