using System.Collections.Generic;
using Fractural.Tasks;

public abstract class DummyScenario : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario002.tscn";

	public override int ScenarioNumber => 5;
	public override string Name => "TODO";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [];

	public override string IntroductionText =>
		"""
		TODO
		""";

	public override string ConclusionText =>
		"""
		TODO
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		//TODO
	];

	public override List<SavedReward> Rewards { get; } =
	[
		//TODO
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
	}
}

public class Scenario031 : ScenarioModel
{
	public override int ScenarioNumber => 31;

	public override string ScenePath => "res://Content/Scenarios/Scenario002.tscn";
	public override string Name => "TODO";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [];

	public override string IntroductionText =>
		"""
		TODO
		""";

	public override string ConclusionText =>
		"""
		TODO
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		//TODO
	];

	public override List<SavedReward> Rewards { get; } =
	[
		//TODO
	];
}

public class Scenario032 : ScenarioModel
{
	public override int ScenarioNumber => 32;

	public override string ScenePath => "res://Content/Scenarios/Scenario002.tscn";
	public override string Name => "TODO";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [];

	public override string IntroductionText =>
		"""
		TODO
		""";

	public override string ConclusionText =>
		"""
		TODO
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		//TODO
	];

	public override List<SavedReward> Rewards { get; } =
	[
		//TODO
	];
}

public class Scenario034 : ScenarioModel
{
	public override int ScenarioNumber => 34;

	public override string ScenePath => "res://Content/Scenarios/Scenario002.tscn";
	public override string Name => "TODO";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [];

	public override string IntroductionText =>
		"""
		TODO
		""";

	public override string ConclusionText =>
		"""
		TODO
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		//TODO
	];

	public override List<SavedReward> Rewards { get; } =
	[
		//TODO
	];
}