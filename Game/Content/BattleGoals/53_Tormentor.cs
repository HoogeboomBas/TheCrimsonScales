using System.Linq;
using Fractural.Tasks;

public class Tormentor : TheCrimsonScalesBattleGoal
{
	public override string Title => "Tormentor";
	public override string Description => "Apply a different negative condition to an enemy that already has one or more negative conditions.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ConditionAddedEvent.Subscribe(this,
			parameters =>
				parameters.Target.EnemiesWith(character) &&
				parameters.PotentialConditionGiver == character &&
				parameters.Target.Conditions.Select(condition => condition.ConditionModel)
											.Except([parameters.ConditionModel])
											.Any(conditionModel => conditionModel.IsNegative),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}