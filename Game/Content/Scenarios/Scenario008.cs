using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario008 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario008.tscn";

	public override int ScenarioNumber => 8;
	public override string Name => "Shattered Fortress";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario009>(true)];

	public override string IntroductionText =>
		"""
		You’ve heard of the fortress of course. Infamous in Gloomhaven for having secret rooms that have never been found (or looted), you had always meant to put that right, but somehow never found the time. This seems like the perfect excuse.

		What was supposed to be a one-day journey extended into two, but you find yourself well rested after having camped outside the entrance. You grab the verdigrised bronze handles on the large iron doors, but the gate doesn't budge. Locked. Odd for an abandoned building. Grumbling, you take your weapon out and bash on the door. It takes some work, but eventually the decayed hinges give way, and the door falls from the jamb.

		You’re greeted by an overbearing automaton directly ahead, and two whirring guns to your left, pointed in your direction. You suspected that the fortress was guarded, but this level of security and the locked door, suggests that it is still maintained. You don’t have time to wonder who or why is behind this though, as the golem’s eyes begin to glow red and it slowly starts to animate. “Kill the intruder. Kill the intruder,” you hear the golem chant in a robotic monotone. You hear the gears turning as the stone-and-metal automaton raises its hefty arms into a fighting posture, and starts to advance; there’s no turning back now...
		""";

	public override string ConclusionText =>
		"""
		With the last of the robotic security devices dismantled, you hear a click in the back of the room. There’s no captive in sight, but you see a previously hidden doorway open in the distance with a long set of stairs heading down.

		It seems like the rumors were at least partly true—and this doesn’t seem to be over quite yet.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<AncientArtillery>(),
		ModelDB.Monster<LivingSpirit>(),
		ModelDB.Monster<StoneGolem>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveItemReward(ModelDB.Item<CuriousPendant>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario009>())
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<DrakesBlood>());
	}
}