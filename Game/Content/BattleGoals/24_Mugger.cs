using System.Collections.Generic;
using Fractural.Tasks;

public class Mugger : TheCrimsonScalesBattleGoal
{
	public override string Title => "Mugger";
	public override string Description => "Kill an enemy and loot the loot token it drops in the same round.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		List<Figure> roundKilledFigures = new List<Figure>();
		Dictionary<Coin, Figure> roundCoinsToCoinDroppersMap = [];

		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialKiller == character,
			async parameters =>
			{
				roundKilledFigures.Add(parameters.Figure);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.CoinSpawnedEvent.Subscribe(character, this,
			parameters => parameters.PotentialDropper != null,
			async parameters =>
			{
				roundCoinsToCoinDroppersMap.Add(parameters.Coin, parameters.PotentialDropper);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.CoinLootedEvent.Subscribe(character, this,
			parameters =>
				roundCoinsToCoinDroppersMap.ContainsKey(parameters.Coin) && 
				roundKilledFigures.Contains(roundCoinsToCoinDroppersMap[parameters.Coin]),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(character, this,
			parameters => true,
			async parameters =>
			{
				roundKilledFigures.Clear();
				roundCoinsToCoinDroppersMap.Clear();

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}