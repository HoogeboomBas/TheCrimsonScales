using Fractural.Tasks;

public class Conservator : TheCrimsonScalesBattleGoal
{
	public override string Title => "Conservator";
	public override string Description => "Never perform an action with a lost icon.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Performer == character && 
				parameters.AbilityCardSide.Model.Loss,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		await GDTask.CompletedTask;
	}
}