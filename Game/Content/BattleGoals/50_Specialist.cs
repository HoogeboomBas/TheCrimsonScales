using Fractural.Tasks;

public class Specialist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Specialist";
	public override string Description => "Never perform a basic action.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Performer == character && 
				parameters.AbilityCardSide.AbilityCardSideType is AbilityCardSideType.BasicTop or AbilityCardSideType.BasicBottom &&
				parameters.Performed,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		await GDTask.CompletedTask;
	}
}