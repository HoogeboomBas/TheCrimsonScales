using Fractural.Tasks;

public class Prepper : TheCrimsonScalesBattleGoal
{
	public override string Title => "Prepper";
	public override string Description => "Perform no attack abilities in the first three rounds.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => _failed;

	public override int MaxProgress => 3;

	private bool _failed = false;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		_failed = false;

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => !battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this,
			parameters => 
				parameters.Performer == character && 
				!battleGoal.ProgressFull,
			async parameters =>
			{
				_failed = true;
				battleGoal.AdjustProgress(MaxProgress - battleGoal.Progress);

				await GDTask.CompletedTask; 
			}
		);

		await GDTask.CompletedTask;
	}
}