using Newtonsoft.Json.Linq;

public class CampaignSaveFile : SaveFile<CampaignSaveData>
{
	public bool RemovedSavedScenario { get; set; }

	protected override Migration[] Migrations { get; } =
	[
		new Migration001(),
		new Migration002(),
		new Migration003(),
		new Migration004(),
	];

	public CampaignSaveFile(string path)
		: base(path)
	{
	}

	protected override void Migrate(JObject saveData)
	{
		base.Migrate(saveData);

		RemovedSavedScenario = false;
		if(saveData.GetValue("AppVersion")?.ToObject<string>() != GetVersion())
		{
			if(saveData.TryGetValue("SavedCampaign", out JToken savedCampaign))
			{
				JObject savedCampaignObject = (JObject)savedCampaign;
				if(savedCampaignObject.ContainsKey("SavedScenario"))
				{
					savedCampaignObject.Remove("SavedScenario");
					RemovedSavedScenario = true;
				}
			}
		}
	}
}