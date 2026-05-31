using Fractural.Tasks;

public class Gambler : TheCrimsonScalesBattleGoal
{
	public override string Title => "Gambler";
	public override string Description => "Kill an enemy with an attack that has disadvantage.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure.EnemiesWith(character) &&
				parameters.PotentialKiller == character &&
				parameters.PotentialAbilityState is AttackAbility.State state &&
				state.SingleTargetHasDisadvantage,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}