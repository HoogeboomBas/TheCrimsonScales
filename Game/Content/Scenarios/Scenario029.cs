using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario029 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario029.tscn";
	public override int ScenarioNumber => 29;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<TaintedScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario030>(true)];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new KillSpecificEnemiesTypeGoals(ModelDB.Monster<BoneArcher>(), "Kill all Bone Archers to win this scenario");

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		//TODO: Scenario Effect

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<ChainMace>());

		UpdateScenarioText(
			$"The Living Bones are the Bone Archers. They gain {Icons.Inline(Icons.Range)}3 on all attacks and only have {Icons.Inline(Icons.Targets)}1.");
	}
}