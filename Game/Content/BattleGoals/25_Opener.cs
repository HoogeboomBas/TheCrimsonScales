using Fractural.Tasks;

public class Opener : TheCrimsonScalesBattleGoal
{
	public override string Title => "Opener";
	public override string Description => "Kill the first enemy to die in the scenario.";

	public override bool FailIfProgressFull => _failed;

	private bool _failed = false;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		_failed = false;
	
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => !battleGoal.ProgressFull && parameters.Figure.EnemiesWith(character),
			async parameters =>
			{
				if(parameters.PotentialKiller != character)
				{
					_failed = true;
				}

				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;

	}
}