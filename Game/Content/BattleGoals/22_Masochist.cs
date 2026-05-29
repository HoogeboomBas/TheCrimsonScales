using Fractural.Tasks;

public class Masochist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Masochist";
	public override string Description => "End the scenario with a hit point value of 3 or less.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ScenarioEndedEvent.Subscribe(this,
			parameters => character.Health <= 3,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			});

		await GDTask.CompletedTask;
	}
}