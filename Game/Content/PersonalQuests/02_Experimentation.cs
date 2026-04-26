using Fractural.Tasks;

public class Experimentation : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Experimentation";
	public override ClassModel ClassToUnlock => ModelDB.Class<BrightsparkModel>();
	public override int MaxProgress => 30;
	protected override int AtlasIndex => 2;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(character, this,
			parameters =>
				parameters.Performer == character &&
				parameters.ResultingState.IsLoss(),
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}