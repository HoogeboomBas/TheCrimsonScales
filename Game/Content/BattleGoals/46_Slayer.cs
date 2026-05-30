using Fractural.Tasks;

public class Slayer : TheCrimsonScalesBattleGoal
{
	public override string Title => "Slayer";
	public override string Description => "Kill two or more enemies in the same round.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override int MaxProgress => 2;
	
	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.Figure.EnemiesWith(character) &&
				parameters.PotentialKiller == character &&
				!battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => !battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.ResetProgress();

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}