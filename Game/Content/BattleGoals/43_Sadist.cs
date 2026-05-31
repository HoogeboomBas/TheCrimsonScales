using Fractural.Tasks;

public class Sadist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Sadist";
	public override string Description => "Kill five or more enemies.";

	public override int MaxProgress => 5;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure.EnemiesWith(character) &&
				parameters.PotentialKiller == character,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}