using Fractural.Tasks;

public class PredatorAndPrey : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Predator and Prey";
	public override ClassModel ClassToUnlock => ModelDB.Class<LuminaryModel>();
	public override int MaxProgress => 15;
	protected override int AtlasIndex => 15;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		int killCount = 0;

		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
			parameters => parameters.PotentialKiller == character,
			async parameters =>
			{
				killCount++;

				if(killCount == 2)
				{
					personalQuestData.AdjustProgress(1, character);
				}

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(character, this,
			parameters => true,
			async parameters =>
			{
				killCount = 0;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(character, this,
			parameters => true,
			async parameters =>
			{
				killCount = 0;

				await GDTask.CompletedTask;
			}
		);
	}
}