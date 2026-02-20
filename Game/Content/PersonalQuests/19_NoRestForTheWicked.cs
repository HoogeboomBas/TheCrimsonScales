using Fractural.Tasks;

public class NoRestForTheWicked : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "No Rest for the Wicked";
	public override ClassModel ClassToUnlock => ModelDB.Class<SpiritCallerModel>();
	public override int MaxProgress => 10;
	protected override int AtlasIndex => 19;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		bool longRested = false;
		bool shortRested = false;

		ScenarioEvents.LongRestStartedEvent.Subscribe(character, this,
			parameters => parameters.Character == character,
			async parameters =>
			{
				longRested = true;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.ShortRestStartedEvent.Subscribe(character, this,
			parameters => parameters.Character == character,
			async parameters =>
			{
				shortRested = true;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.ScenarioEndedEvent.Subscribe(character, this,
			parameters =>
				parameters.Win &&
				!(longRested && shortRested),
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}