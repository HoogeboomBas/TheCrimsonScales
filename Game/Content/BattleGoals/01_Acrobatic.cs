using Fractural.Tasks;

public class Acrobatic : TheCrimsonScalesBattleGoal
{
	public override string Title => "Acrobatic";
	public override string Description => "Lose a card to negate suffering 5 or more damage.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.LosingCardToNegateDamageEvent.Subscribe(this,
			parameters =>
				parameters.Character == character &&
				parameters.SufferDamageParameters.CalculatedCurrentDamage >= 5,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}