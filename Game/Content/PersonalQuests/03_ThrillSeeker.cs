using Fractural.Tasks;

public class ThrillSeeker : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Thrill Seeker";
	public override ClassModel ClassToUnlock => ModelDB.Class<BrightsparkModel>();
	public override int MaxProgress => 20;
	protected override int AtlasIndex => 3;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.RoomRevealedEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialOpener == character,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}