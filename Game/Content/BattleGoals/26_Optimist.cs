using Fractural.Tasks;

public class Optimist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Optimist";
	public override string Description => "Remove a negative condition from yourself or an ally two or more times.";

	public override int MaxProgress => 2;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AfterRemoveConditionEvent.Subscribe(this,
			parameters => 
				!battleGoal.ProgressFull &&
				(parameters.Figure == character || parameters.Figure.AlliedWith(character)) &&
				parameters.PotentialAbilityState?.Authority == character &&
				parameters.Condition.IsNegative,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}