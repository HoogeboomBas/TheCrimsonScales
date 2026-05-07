using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario001 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario001.tscn";

	public override int ScenarioNumber => 1;
	public override string Name => "The Dark Lake";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario002>(true)];

	public override string IntroductionText =>
		"""
		Following the vague instructions you were given, you head to the Dark Lake. As you arrive at the shore you see... nothing. The shore, and the lake are deserted of all wildlife, let alone ‘strange creatures.’ You start to walk round the lake, growing slowly more dispirited as you go. You’ve clearly been tricked, and accusing looks pass between the party as you mutter about whose fault it was.

		Suddenly, the still surface of the lake is disturbed. The water rises and twists in two frothing columns which settle into large spirit figures you’ve never seen before. Almost at the same time a squad of Vermling Scouts appear out of the undergrowth and, signaling to each other, attempt to surround the water spirits before battling them.

		Splashing in the shallows, your party is transfixed by the two forces maneuvering this way and that until, suddenly, you realize that you are caught in between them.

		Realizing simultaneously that these may be the strange creatures you are looking for, and that you’ve been caught in the middle of a nasty fight, you realize there’s one solution. Kill them all.
		""";

	public override string ConclusionText =>
		"""
		Wet, tired, and bloody, you look around at where you find yourself. The water spirits definitely counted as strange creatures, and they seemed to be guarding something.

		You explore further, and find an old tunnel. It is obviously some sort of overflow from somewhere, but currently it is only damp. There are prints in the muddy soil by the entrance though, and you can see other signs of activity. Maybe, whatever this mysterious object is, it lies in here.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<SpittingDrake>(),
		ModelDB.Monster<VermlingScout>(),
		ModelDB.Monster<WaterSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario002>())
	];

	public override string BGSPath => "res://Audio/BGS/Forest Day.ogg";

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<DizzyingTincture>());
	}
}