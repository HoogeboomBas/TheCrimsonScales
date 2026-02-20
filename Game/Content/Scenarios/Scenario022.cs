using System.Linq;
using Fractural.Tasks;

public class Scenario022 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario022.tscn";
	public override int ScenarioNumber => 22;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new CustomScenarioGoals("");

	private int _remainingImpKills;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		//Scenario Effect

		_remainingImpKills = GameController.Instance.SavedCampaign.Characters.Count * 4;

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 15).SetItemLoot(ModelDB.Item<ShiftingCompass>());
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 32).SetItemLoot(ModelDB.Item<CuriousPendant>());

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Monster monster && monster.MonsterModel is Imp && _remainingImpKills > 0,
			async parameters =>
			{
				_remainingImpKills--;
				UpdateScenarioText();
				await GDTask.CompletedTask;
			});
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => _remainingImpKills == 0,
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			});
		UpdateScenarioText();
	}

	private void UpdateScenarioText()
	{
		GameController.Instance.SpecialRulesView.SetText($"Kill {_remainingImpKills} more Imps to win this scenario.");
	}
}