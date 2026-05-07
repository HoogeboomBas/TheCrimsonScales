public class NewCampaignSceneRequest : SceneRequest
{
	public int CampaignIndex { get; }

	public override string ScenePath => "res://Scenes/NewCampaign/NewCampaign.tscn";

	public NewCampaignSceneRequest(int campaignIndex)
	{
		CampaignIndex = campaignIndex;
	}
}