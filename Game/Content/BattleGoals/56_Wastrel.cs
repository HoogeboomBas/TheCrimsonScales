using Fractural.Tasks;

public class Wastrel : TheCrimsonScalesBattleGoal
{
	public override string Title => "Wastrel";
	public override string Description => "Lose a card to negate 2 or less damage from an attack.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.LosingCardToNegateDamageEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Character == character &&
				parameters.SufferDamageParameters.FromAttack &&
				parameters.SufferDamageParameters.CalculatedCurrentDamage <= 2,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}