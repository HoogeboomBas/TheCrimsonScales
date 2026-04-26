// using System.Collections.Generic;
// using System.Linq;
// using Fractural.Tasks;
//
// public class Scenario018 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario018.tscn";
// 	public override int ScenarioNumber => 18;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
// 	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario020>(), new ScenarioConnection<Scenario021>()];
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new CustomScenarioGoals(
// 			$"Kill all enemies and have all characters occupy hexes {Icons.InlineMarker(Marker.Type.a)} or exhaust on a hex {Icons.InlineMarker(Marker.Type.a)} to win this scenario.");
//
// 	private IEnumerable<Hex> _markerHexes;
//
// 	public override async GDTask StartOfScenarioEffects(Character character)
// 	{
// 		await AbilityCmd.AddCondition(null, character, Conditions.Immobilize);
// 	}
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		UpdateScenarioText($"If any character is exhausted while not occupying a hex {Icons.InlineMarker(Marker.Type.a)}, the scenario is lost");
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<BootsOfPerpetuity>());
//
// 		_markerHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex);
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => GameController.Instance.Map.Figures.Where(figure => figure is Character)
// 				.All(character => _markerHexes.Contains(character.Hex)),
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Win();
// 			});
//
// 		ScenarioEvents.FigureKilledEvent.Subscribe(this,
// 			parameters => parameters.Figure is Character && !_markerHexes.Contains(parameters.Figure.Hex),
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Lose();
// 			}
// 		);
// 	}
// }

