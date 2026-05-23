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

public class Scenario025 : ScenarioModel
{
	public override int ScenarioNumber => 25;

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

public class Scenario035 : ScenarioModel
{
	public override int ScenarioNumber => 35;

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

public class Scenario042 : ScenarioModel
{
	public override int ScenarioNumber => 42;

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

public class Scenario043 : ScenarioModel
{
	public override int ScenarioNumber => 43;

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

public class Scenario044 : ScenarioModel
{
	public override int ScenarioNumber => 44;

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

public class Scenario045 : ScenarioModel
{
	public override int ScenarioNumber => 45;

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

public class Scenario046 : ScenarioModel
{
	public override int ScenarioNumber => 46;

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

public class Scenario047 : ScenarioModel
{
	public override int ScenarioNumber => 47;

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

public class Scenario048 : ScenarioModel
{
	public override int ScenarioNumber => 48;

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

public class Scenario049 : ScenarioModel
{
	public override int ScenarioNumber => 49;

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

public class Scenario050 : ScenarioModel
{
	public override int ScenarioNumber => 50;

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

public class Scenario051 : ScenarioModel
{
	public override int ScenarioNumber => 51;

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

public class Scenario052 : ScenarioModel
{
	public override int ScenarioNumber => 52;

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

public class Scenario053 : ScenarioModel
{
	public override int ScenarioNumber => 53;

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

public class Scenario054 : ScenarioModel
{
	public override int ScenarioNumber => 54;

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

public class Scenario055 : ScenarioModel
{
	public override int ScenarioNumber => 55;

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