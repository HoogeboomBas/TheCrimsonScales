using Fractural.Tasks;

public class Vanguard : TheCrimsonScalesBattleGoal
{
	public override string Title => "Vanguard";
	public override string Description => "Never attack an enemy that has already acted in the round.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;
	
	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(character, this,
			parameters => 
				parameters.Performer == character &&
				parameters.AbilityState.Target.DidTakeTurn,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}