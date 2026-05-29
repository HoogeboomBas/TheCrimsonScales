using Fractural.Tasks;

public class Closer : TheCrimsonScalesBattleGoal
{
	public override string Title => "Closer";
	public override string Description => "Kill the last enemy to die in the scenario.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		bool lastToDieKilledByThisCharacter = false;

		ScenarioEvents.ScenarioEndedEvent.Subscribe(this,
			parameters => lastToDieKilledByThisCharacter,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			});

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure.EnemiesWith(character),
			async parameters =>
			{
				if(parameters.PotentialKiller == character)
				{
					lastToDieKilledByThisCharacter = true;
				}
				else
				{
					lastToDieKilledByThisCharacter = false;
				}

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}