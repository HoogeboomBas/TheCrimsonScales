using Fractural.Tasks;

public class Overachiever : TheCrimsonScalesBattleGoal
{
	public override string Title => "Overachiever";
	public override string Description => "Kill an enemy and open a door in the same turn, in either order.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override int MaxProgress => 2;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		bool doorOpened = false;
		bool enemyKilled = false;

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character,
			async parameters =>
			{
				battleGoal.ResetProgress();

				doorOpened = false;
				enemyKilled = false;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.DoorOpenedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.PotentialOpener == character &&
				!doorOpened,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				doorOpened = true;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.PotentialKiller == character &&
				parameters.Figure.EnemiesWith(character) &&
				!enemyKilled,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				enemyKilled = true;

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}