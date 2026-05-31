using Fractural.Tasks;

public class Straggler : TheCrimsonScalesBattleGoal
{
	public override string Title => "Straggler";
	public override string Description => "Never short rest.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Character == character,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}