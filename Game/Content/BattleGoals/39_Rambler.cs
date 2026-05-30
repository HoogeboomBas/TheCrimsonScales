using Fractural.Tasks;

public class Rambler : TheCrimsonScalesBattleGoal
{
	public override string Title => "Rambler";
	public override string Description => "End no more than three of your turns in the hex in which you started the turn, except when long resting.";

	public override int MaxProgress => 4;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		Hex startingHex = null;

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
			parameters => parameters.Figure == character,
			async parameters =>
			{
				startingHex = character.Hex;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters => 
				parameters.Figure == character && 
				character.Hex == startingHex && 
				!character.LongResting,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}