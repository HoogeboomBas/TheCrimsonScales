using Fractural.Tasks;

public class Pincushion : TheCrimsonScalesBattleGoal
{
	public override string Title => "Pincushion";
	public override string Description => "Be targeted by attacks from three or more enemies in the same round.";

	public override int MaxProgress => 3;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.AbilityState.Target == character &&
				parameters.Performer.EnemiesWith(character),
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