using Fractural.Tasks;

public class Slowpoke : TheCrimsonScalesBattleGoal
{
	public override string Title => "Slowpoke";
	public override string Description => "Move no more than two hexes on each turn.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => true;

	public override int MaxProgress => 3;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.ResetProgress();

				await GDTask.CompletedTask; 
			}
		);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
			parameters => 
				parameters.Figure == character &&
					(parameters.PotentialAbilityState is MoveAbility.State ||
					(parameters.PotentialAbilityState is TargetedAbilityState && parameters.ForcedMovement)),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		await GDTask.CompletedTask;
	}
}