using System.Collections.Generic;
using Fractural.Tasks;

public class AdrenalineSpike : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Adrenaline Spike";
	public override ClassModel ClassToUnlock => ModelDB.Class<FireKnightModel>();
	public override int MaxProgress => 15;
	protected override int AtlasIndex => 8;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

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
				personalQuestData.AdjustProgress(1, character);

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
	}
}