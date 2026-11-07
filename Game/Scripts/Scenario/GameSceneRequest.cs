public class GameSceneRequest : SceneRequest
{
	public SavedCampaign SavedCampaign { get; }
	public bool FromUndo { get; }

	public override string ScenePath => "res://Scenes/Scenario/Game.tscn";

	public GameSceneRequest(SavedCampaign savedCampaign, bool fromUndo = false)
	{
		SavedCampaign = savedCampaign;
		FromUndo = fromUndo;
	}
}