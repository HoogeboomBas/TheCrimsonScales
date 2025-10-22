using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class Migrator
{
	private static readonly Migration[] Migrations =
	[
		new Migration001(),
		new Migration002(),
		new Migration003(),
	];

	public static int MigrationVersion => Migrations.Length;

	public static string Migrate(string json, string currentAppVersion, out bool removedSavedScenario)
	{
		JObject saveData = JObject.Parse(json);
		int migrationVersion = saveData.GetValue("MigrationVersion")?.ToObject<int>() ?? 0;

		for(int i = migrationVersion; i < Migrations.Length; i++)
		{
			Log.Write($"Migrating save file from version {i} to {i + 1}.");
			Migrations[i].Migrate(saveData);
		}

		saveData["MigrationVersion"] = MigrationVersion;

		removedSavedScenario = false;
		if(saveData.GetValue("AppVersion")?.ToObject<string>() != currentAppVersion)
		{
			if(saveData.TryGetValue("SavedCampaign", out JToken savedCampaign))
			{
				JObject savedCampaignObject = (JObject)savedCampaign;
				if(savedCampaignObject.ContainsKey("SavedScenario"))
				{
					savedCampaignObject.Remove("SavedScenario");
					removedSavedScenario = true;
				}
			}
		}

		json = JsonConvert.SerializeObject(saveData, SaveFile.JsonSerializerSettings);

		return json;
	}
}