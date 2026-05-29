using Fractural.Tasks;

public class Executioner : TheCrimsonScalesBattleGoal
{
	public override string Title => "Executioner";
	public override string Description => "Kill an undamaged enemy with a single attack action.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
			parameters => parameters.Figure.EnemiesWith(character) &&
							parameters.PotentialAbilityState is AttackAbility.State &&
							parameters.PotentialAbilityState.Performer == character &&
							parameters.DamageSuffered >= parameters.Figure.MaxHealth,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}