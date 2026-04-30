using System.Linq;
using Fractural.Tasks;

public class Contagious : TheCrimsonScalesBattleGoal
{
	public override string Title => "Contagious";
	public override string Description => "While afflicted by a negative condition, apply any negative condition to an enemy.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ConditionAddedEvent.Subscribe(this,
			parameters =>
				parameters.PotentialConditionGiver == character &&
				character.EnemiesWith(parameters.Target) &&
				character.Conditions.Any(condition => condition.ConditionModel.IsNegative),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}