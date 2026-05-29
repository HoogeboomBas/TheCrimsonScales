using Fractural.Tasks;

public class Scrambler : TheCrimsonScalesBattleGoal
{
	public override string Title => "Scrambler";
	public override string Description => "Never long rest.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.LongRestStartedEvent.Subscribe(this,
			parameters => parameters.Character == character,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}