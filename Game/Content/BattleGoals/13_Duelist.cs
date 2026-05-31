using System.Linq;
using Fractural.Tasks;

public class Duelist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Duelist";
	public override string Description => "Never exit a hex adjacent to the enemy except through forced movement.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureExitingHexEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character && 
				parameters.PotentialAbilityState is MoveAbility.State or TeleportAbility.State &&
				parameters.Hex.Neighbours.Any(hex => hex.GetFigures().Any(figure => figure.EnemiesWith(character))),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		await GDTask.CompletedTask;
	}
}