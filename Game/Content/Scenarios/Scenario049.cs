using Fractural.Tasks;

public class Scenario049 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario049.tscn";
	public override int ScenarioNumber => 49;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		UpdateScenarioText("All City Guards and City Archers are allies to you and to each other and enemies to all other monsters.");
	}
}