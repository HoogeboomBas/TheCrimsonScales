using Newtonsoft.Json.Linq;

public class DummyMigration : Migration
{
	public override void Migrate(JObject saveData)
	{
		// Doesn't change the save file
	}
}