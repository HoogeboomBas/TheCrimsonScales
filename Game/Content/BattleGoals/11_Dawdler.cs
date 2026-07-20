using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Dawdler : TheCrimsonScalesBattleGoal
{
	public override string Title => "Dawdler";
	public override string Description => "Never use your lowest initiative played card as your initiative card.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.RoundStartedBeforeInitiativesSortedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				!character.IsDead &&
				!character.LongResting &&
				character.RoundCards[0].Model.Initiative < character.RoundCards[1].Model.Initiative,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}