using Fractural.Tasks;

public class Insomniac : TheCrimsonScalesBattleGoal
{
	public override string Title => "Insomniac";
	public override string Description => "Suffer damage from an attack in the same round you long rest.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character &&
				parameters.PotentialAbilityState is AttackAbility.State &&
				character.LongResting,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(this);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}