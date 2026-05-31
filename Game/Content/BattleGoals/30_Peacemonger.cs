using Fractural.Tasks;

public class Peacemonger : TheCrimsonScalesBattleGoal
{
	public override string Title => "Peacemonger";
	public override string Description => "Never kill an enemy.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => true;

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