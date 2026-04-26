// using System.Linq;
// using Fractural.Tasks;
//
// public class Scenario048 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario048.tscn";
// 	public override int ScenarioNumber => 48;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new CustomScenarioGoals("Extinguish all fires to win this scenario");
//
// 	//TODO: Remove the final room for 2 players
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		if(GameController.Instance.SavedCampaign.Characters.Count >= 3)
// 		{
// 			foreach(Marker marker in GameController.Instance.Map.GetMarkers(Marker.Type.a))
// 			{
// 				marker.GetHexObject<DifficultTerrain>()?.Destroy();
// 			}
// 		}
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => !GameController.Instance.Map.Rooms.SelectMany(room => room.GetChildrenOfType<DifficultTerrain>()).Any(),
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Win();
// 			});
//
// 		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
// 			parameters => parameters.Figure is Character &&
// 			              (parameters.Figure.Hex.GetHexObjectOfType<DifficultTerrain>()?.Name.ToString().Contains("HotCoalsDifficultTerrain1H") ??
// 			               false),
// 			async parameters =>
// 			{
// 				await parameters.Figure.Hex.GetHexObjectOfType<DifficultTerrain>().Destroy();
// 			}, EffectType.Selectable,
// 			effectButtonParameters: new IconEffectButton.Parameters("res://Art/OverlayTiles/Hot Coals 1h.png"),
// 			effectInfoViewParameters: new TextEffectInfoView.Parameters("Extinguish fire (Remove hot coals from the board)"));
//
// 		//TODO: Either use normal hotcoals and have characters treat them differently, or use difficult terrain ones and have monster treat them differently.
// 	}
// }

