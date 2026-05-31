using System.Collections.Generic;
using Fractural.Tasks;

public class Assistant : TheCrimsonScalesBattleGoal
{
	public override string Title => "Assistant";
	public override string Description => "Kill an enemy attacked by any of your allies earlier in the round.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		List<Figure> attackedFigures = [];

		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				character.AlliedWith(parameters.Performer),
			async parameters =>
			{
				attackedFigures.AddIfNew(parameters.AbilityState.Target);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => !battleGoal.ProgressFull,
			async parameters =>
			{
				attackedFigures.Clear();

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.PotentialKiller == character &&
				character.EnemiesWith(parameters.Figure) &&
				attackedFigures.Contains(parameters.Figure),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}