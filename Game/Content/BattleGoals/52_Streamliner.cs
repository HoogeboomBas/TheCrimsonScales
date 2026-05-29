using System.Linq;
using Fractural.Tasks;

public class Streamliner : TheCrimsonScalesBattleGoal
{
	public override string Title => "Streamliner";
	public override string Description => "Have five or more total cards in your hand and discard pile at the end of the scenario.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ScenarioEndedEvent.Subscribe(this,
			parameters => character.Cards.Count(card => card.CardState == CardState.Hand || card.CardState == CardState.Discarded) > 5,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			});

		await GDTask.CompletedTask;
	}
}