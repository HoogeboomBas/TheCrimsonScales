using System.Linq;
using Fractural.Tasks;

public class Zealot : TheCrimsonScalesBattleGoal
{
	public override string Title => "Zealot";

	public override string Description =>
		"Have three or fewer total cards in your hand and discard pile while also not exhausted at the end of the scenario.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ScenarioEndedEvent.Subscribe(this,
			parameters => 
				!character.IsDead &&
				character.Cards.Count(card => card.CardState == CardState.Hand || card.CardState == CardState.Discarded) <= 3,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			});

		await GDTask.CompletedTask;
	}
}