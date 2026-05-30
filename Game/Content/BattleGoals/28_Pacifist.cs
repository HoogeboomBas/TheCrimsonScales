using Fractural.Tasks;

public class Pacifist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Pacifist";
	public override string Description => "Kill three or fewer enemies.";

	public override int MaxProgress => 4;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => 
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