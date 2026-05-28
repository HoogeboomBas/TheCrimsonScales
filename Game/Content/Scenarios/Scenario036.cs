using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario036 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario036.tscn";

	public override int ScenarioNumber => 36;
	public override string Name => "Hunter’s Bounty";

	protected override List<ScenarioRequirement> Requirements => [new PersonalQuestRequirement(ModelDB.PersonalQuest<BanditBanisher>())];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();

	public override string IntroductionText =>
		"""
		You look at the fearsome Chainguard who has just left the prison’s employment. “What are you going to do now?”

		The Chainguard laughs darkly “I have a couple of loose ends to tie up.” Looking you up and down, as if weighing a decision, he growls, “I could use a hand, if you want to earn some gold?” You don’t need to be asked twice.

		“Two former prisoners ravaged an Inox settlement and left nothing but bodies. There’s a big price on their heads. Unfortunately for them, they weren’t subtle about their plans for when they left prison. They’ve headed to the Corpsewood to reunite with their gang. Help me take them down, and I’ll split the gold with you.”

		You ask who is it you’re after, and the Chainguard snarls, “the ‘Terrible Two’ they call themselves, a pair of brothers who think they’re special.”

		You ask what’s so terrible about them, and a twisted smile forms on the Chainguard’s face. “You’ll see,” says the Chainguard before rolling his shoulders back and striding off towards the Corpsewood.
		""";

	public override string ConclusionText =>
		"""
		The brothers and their gang are vanquished, and for the first time you see an expression approaching tranquility on the face of the Chainguard. “Thank you. My brother Inox will be grateful.” He kneels for a moment in silent reflection at justice delivered, before standing and resuming his usual attitude. “I wasn’t sure about you, but you’ve proven fairly capable. I go where the work is, and the work is whoever I’m paid to hunt down. Join me, and there’ll be more of this,” he says, throwing you a bag of gold he’s cut from one of the bandits. You are temporarily lost for words, but you think he’s just asked to join your party.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BanditArcher>(),
		ModelDB.Monster<BanditGuard>(),
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<TerribleTwoBanditArcher>(),
		ModelDB.Monster<TerribleTwoBanditArcher>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new OpenEnvelopeReward(ModelDB.PersonalQuest<BanditBanisher>()),
		new GainGoldEachReward(10)
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<TerribleTwoBanditArcher>()));
		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<TerribleTwoBanditGuard>()));

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 2).SetItemLoot(ModelDB.Item<HookShot>());
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 30).SetItemLoot(ModelDB.Item<BonecladShawl>());
	}
}