using Newtonsoft.Json.Linq;

public class Migration004 : Migration
{
	public override void Migrate(JObject saveData)
	{
		// Set the ItemModelId of all saved items
		if(saveData.TryGetValue("SavedCampaign", out JToken savedCampaign))
		{
			JToken savedItems = savedCampaign["SavedItems"];
			if(savedItems != null)
			{
				JObject savedItemsDictionary = (JObject)savedItems;
				foreach((string key, JToken itemValues) in savedItemsDictionary)
				{
					JObject itemValuesDictionary = (JObject)itemValues;
					itemValuesDictionary.Add("ItemModelId", key);
				}
			}
		}
	}
}