using Fractural.Tasks;

public class Ravager : TheCrimsonScalesBattleGoal
{
	public override string Title => "Ravager";
	public override string Description => "Perform two actions with lost icons in the same turn.";

	public override int MaxProgress => 2;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AbilityCardSideEndedEvent.Subscribe(this,
			parameters => parameters.Performer == character && 
							parameters.AbilityCardSide.Model.Loss &&
							!battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters => parameters.Figure == character && !battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.ResetProgress();

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}