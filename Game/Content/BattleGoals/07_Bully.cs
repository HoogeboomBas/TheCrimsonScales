using Fractural.Tasks;

public class Bully : TheCrimsonScalesBattleGoal
{
	public override string Title => "Bully";
	public override string Description => "Kill an enemy afflicted by a negative condition.";

	public override int MaxProgress => 1;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.PotentialKiller == character &&
				parameters.Figure.Conditions.Count > 0,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}