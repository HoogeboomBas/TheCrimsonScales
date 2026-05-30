using Fractural.Tasks;

public class Daredevil : TheCrimsonScalesBattleGoal
{
	public override string Title => "Daredevil";
	public override string Description => "Add two cards to your lost pile before your first rest.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override int MaxProgress => 2;

	public override bool FailIfProgressFull => _failed;

	private bool _failed = false;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		_failed = false;

		ScenarioEvents.LostCardEvent.Subscribe(this,
			parameters => 
				parameters.Character == character &&
				!battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
			parameters =>
				character == 
					parameters.Character &&
					!battleGoal.ProgressFull,
			async parameters =>
			{
				_failed = true;
				battleGoal.AdjustProgress(2);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.LongRestStartedEvent.Subscribe(this,
			parameters =>
				character == 
					parameters.Character &&
					!battleGoal.ProgressFull,
			async parameters =>
			{
				_failed = true;
				battleGoal.AdjustProgress(2);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}