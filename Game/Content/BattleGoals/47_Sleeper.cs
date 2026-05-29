using System.Linq;
using Fractural.Tasks;

public class Sleeper : TheCrimsonScalesBattleGoal
{
	public override string Title => "Sleeper";
	public override string Description => "Have one or more cards in your hand each time you rest.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
			parameters =>
				parameters.Character == character &&
				!parameters.Character.Cards.Any(card => card.CardState == CardState.Hand),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.LongRestStartedEvent.Subscribe(this,
			parameters =>
				parameters.Character == character &&
				!parameters.Character.Cards.Any(card => card.CardState == CardState.Hand),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}