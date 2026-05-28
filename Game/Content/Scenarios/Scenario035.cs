using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario035 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario035.tscn";
	public override int ScenarioNumber => 35;

	public override string Name => "Prison Riot";

	protected override List<ScenarioRequirement> Requirements => [new PersonalQuestRequirement(ModelDB.PersonalQuest<BanditBanisher>())];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario036>()];

	public override string IntroductionText =>
		"""
		Having a quiet drink one evening, the relative peace is shattered by a mass scrambling of the slightly rag-tag City Guard, clearly responding to an emergency, but looking more panicked than forceful defenders of the gates. Slightly amused, you take another swig and return to your conversation.

		A few minutes later, noticeably fewer guards sprint back the way they came. There is a snatched shout of “Devils—in the cells!” This does not really qualify as news in Gloomhaven, where even the most upstanding citizens are not exactly angels, but still the inn begins to empty as the patrons filter outside to see what is going on.

		It immediately becomes clear that there is a major disturbance in the direction of the prison—you can hear screaming and there is a large column of smoke. Seeing an opportunity to help, and probably make money along the way (or should that be the other way round?) you saunter up to see what the fuss is about.

		As you approach, you see the situation is more serious than you thought. The prison is infested with demons and the prisoners are rioting. There is no use in attempting to negotiate with these prisoners… most of the prison guards have fled in panic and it is up to you to rid the place of all evil before the prisoners escape and wreak havoc in Gloomhaven.

		A hole has somehow been blown in the side of the prison, and the breach is currently being held by a single Chainguard, the warders of Gloomhaven prison. As he knocks back two escaping prisoners at once, he calls over to you in a gruff voice—“Put down these scum with me and the governor will make it worth your while.” You need no further encouragement.
		""";

	public override string ConclusionText =>
		"""
		As the demons are slain and the braver inmates also dispatched, the rest slink back to their cells and act like they were there all along. “Appreciated” mutters the Chainguard and walks a couple of steps away, before turning back to see you still standing there, awaiting the promised payment. “Fine,” the Chainguard sighs impatiently, “I need to see the Governor anyway.”

		The Chainguard leads you up a flight of steps to an office door and bangs on it. “Y-yes?” answers a voice timidly. “Relax Beetleworth, it’s me” grunts the Chainguard.

		“Oh, very good, very good” answers the Governor, not totally hiding his surprise. There is a shuffling, before a heavy object is slid away from behind the door, the sound of a lock turning and a short portly human emerges, looking rather disheveled. “Good work Sergeant, I knew you had it under control” says the Governor, unconvincingly.

		The Chainguard laughs humorlessly before replying “No thanks to those half-trained deserters you found. I told you not to cut corners, the prison would have fallen without these mercs,” nodding at you. The Governor appears to notice you for the first time, straightens his tie and says pompously “Governor Beetleworth, very pleased to meet you. I owe you my thanks!” “And some gold” the Chainguard says, as you think it.

		“Ah, but of course” says Beetleworth between gritted teeth. “And how can I reward you, Sergeant?” “Forget it!” spits the Chainguard. ”I think it’s safer outside these walls than in. I quit!”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BanditArcher>(),
		ModelDB.Monster<BanditGuard>(),
		ModelDB.Monster<NightDemon>(),
		ModelDB.Monster<WindDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario036>())
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		NPC chainguard = await SpawnNPC(GameController.Instance.Map.GetMarker(Marker.Type.a).Hex, 6 + ScenarioLevel * 3, "Chainguard",
			"res://Content/Classes/Chainguard", 50,
			[
				MoveAbility.Builder().WithDistance(3).Build(),
				AttackAbility.Builder().WithDamage(2).WithPush(2).Build()
			],
			textParameters =>
				$"{Icons.Inline(Icons.Move, textParameters)}3\n{Icons.Inline(Icons.Attack, textParameters)}2, {Icons.Inline(Icons.Push, textParameters, ignoreParametersColor: true)}2");

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 28).SetItemLoot(ModelDB.Item<ChainMace>());
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 40).SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure == chainguard,
			async _ =>
			{
				await AbilityCmd.Lose();
			}
		);

		AddScenarioRule("The Chainguard is an ally to you and an enemy to all monsters.");
		AddScenarioRule("The Chainguard draws from the monster ability deck.");
		AddScenarioRule("If the Chainguard is killed, the scenario is lost.");
// 		UpdateScenarioText(
// 			$"""
// 			 The Chainguard is an ally to you and an enemy to all monsters. He acts on initiative 50 each round, performing "{Icons.Inline(Icons.Move)}3, {Icons.Inline(Icons.Attack)}2, {Icons.Inline(Icons.Push)}2".
// 			 The Chainguard draws from the monster ability deck.
//
// 			 If the Chainguard is killed, the scenario is lost.
// 			 """);
	}
}