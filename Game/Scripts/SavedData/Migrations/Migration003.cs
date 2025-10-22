using Newtonsoft.Json.Linq;

public class Migration003 : Migration
{
	public override void Migrate(JObject saveData)
	{
		// Set the levels of all characters to 1
		if(saveData.TryGetValue("SavedCampaign", out JToken savedCampaign))
		{
			savedCampaign["StartingGroup"] = 0;

			string[] classModelNames =
			[
				"CLASS_MODEL.BOMBARD_MODEL",
				"CLASS_MODEL.CHAINGUARD_MODEL",
				"CLASS_MODEL.FIRE_KNIGHT_MODEL",
				"CLASS_MODEL.HIEROPHANT_MODEL",
				"CLASS_MODEL.MIREFOOT_MODEL"
			];

			JObject savedClasses = new JObject();
			foreach(string classModelName in classModelNames)
			{
				savedClasses.Add(classModelName, new JObject(new JProperty("Unlocked", true), new JProperty("Retired", false)));
			}

			savedCampaign["SavedClasses"] = savedClasses;
		}
	}
}