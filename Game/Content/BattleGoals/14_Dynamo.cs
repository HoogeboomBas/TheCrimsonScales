using Fractural.Tasks;

public class Dynamo : TheCrimsonScalesBattleGoal
{
	public override string Title => "Dynamo";
	public override string Description => "Kill an enemy with an attack that would have caused at least 4 more points of damage than necessary.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure.EnemiesWith(character) &&
				parameters.PotentialAbilityState is AttackAbility.State &&
				parameters.PotentialAbilityState.Performer == character &&
				parameters.DamageDealt >= parameters.DamageSuffered + 4,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}