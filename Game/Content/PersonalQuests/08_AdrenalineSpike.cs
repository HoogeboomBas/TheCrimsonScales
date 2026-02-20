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
		List<Coin> roundCoins = new List<Coin>();

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
			parameters =>
				roundKilledFigures.Contains(parameters.PotentialDropper),
			async parameters =>
			{
				roundCoins.Add(parameters.Coin);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.CoinLootedEvent.Subscribe(character, this,
			parameters =>
				roundCoins.Contains(parameters.Coin),
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
				roundCoins.Clear();

				await GDTask.CompletedTask;
			}
		);
	}
}