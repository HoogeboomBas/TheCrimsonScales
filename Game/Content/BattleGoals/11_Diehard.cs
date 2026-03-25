using Fractural.Tasks;
using Godot;

public class Diehard : TheCrimsonScalesBattleGoal
{
	public override string Title => "Diehard";

	public override string Description =>
		"Never have your hit point value drop below half your maximum hit point value (rounded up).";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
			parameters =>
				parameters.Figure == character,
			async parameters =>
			{
				if(character.Health < Mathf.CeilToInt(character.MaxHealth * 0.5f))
				{
					battleGoal.AdjustProgress(1);

					ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(this);
				}

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}