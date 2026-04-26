using System.Collections.Generic;
using Fractural.Tasks;

public class TestScenario : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/TestScenario.tscn";

	public override int ScenarioNumber => 1;
	public override string Name => "Test Scenario";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string IntroductionText =>
		"""
		TODO
		""";

	public override string ConclusionText =>
		"""
		TODO
		""";

	public override List<MonsterModel> MonsterModels { get; } = [];
	// [
	// 	ModelDB.Monster<SpittingDrake>(),
	// 	ModelDB.Monster<VermlingScout>(),
	// 	ModelDB.Monster<WaterSpirit>(),
	// ];

	public override List<SavedReward> Rewards { get; } =
	[
		new GainGoldEachReward(15)
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<VipertoothDagger>());

		List<Objective> objectives = GameController.Instance.Map.GetChildrenOfType<Objective>();
		int objectiveHealth = 1;
		foreach(Objective objective in objectives)
		{
			objective.Init(objectiveHealth, "Look at this test objective");
		}
	}
}