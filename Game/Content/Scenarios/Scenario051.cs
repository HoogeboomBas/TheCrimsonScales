using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario051 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario051.tscn";

	public override int ScenarioNumber => 51;
	public override string Name => "Rodent Warehouse";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();

	public override string IntroductionText =>
		"""
		It is a fine day, and you are walking through the Old Docks with, as usual, one eye out for potential trouble and the other for opportunity.

		As you round a corner you find a loud argument taking place between one of the warehouse managers and a furious Councilman Raksani. His normally polished exterior has well and truly slipped, and the wealthy merchant is furiously jabbing a chubby, ringed finger at the defensive dock worker.

		“Infested! Absolutely infested! I can’t sell any of it! And what exactly am I paying your extortionate security fees for? Sort this out—I want it spotless by the time I return, and then we will talk about how you are going to compensate me!”

		Councilman Raksani turns on his heel and nearly walks into you. Embarrassed and extremely red in the face, he mumbles some kind of greeting and apology in one, and hurries off. The warehouse manager watches him go, shaking his head slightly and massaging his temples. You ask him lightly if there is a problem, and he turns his gaze towards you.

		“Vermlings!” replies the man. “Couldn’t have picked a worse unit to get into. He’s a bit… particular,” he elaborates, nodding in the direction that Councilman Raksani departed. You look at each other. This definitely qualifies as opportunity.

		You respond that you can clear the warehouse out for him, and that it wouldn’t take too long, as long as he had some extra funds for a short-notice rodent contractor.

		The hassled dock worker reaches inside his cloak and throws you a pouch of gold. “Here you are, and same again if you clear them out before his Lordship gets back”.

		You smile and head inside, when he calls to you: “He thinks everything’s been eaten or something, so help yourself to anything you find lying about too.”

		Your smile grows. This is turning into a very good day
		""";

	public override string ConclusionText =>
		"""
		The Vermlings didn’t give up easily, but you rounded them up in the end. You throw the bodies in the filthy dock, pocket a few nice trinkets you come across and claim your bonus from the (notably more relaxed) warehouse manager.

		You are just leaving when Councilman Raksani returns. Looking at his now unoccupied unit and then at you, he says “By the Oak, I don’t know how you did that, but well done!” Casting a hard look at the dock worker, he continues “I’m glad there’s still some hard-working, honest folk about.” It’s not a phrase you often hear yourself described as, but you’ll take it—and the gold he slips you as a thank you.

		A very good day indeed.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<SunDemon>(),
		ModelDB.Monster<VermlingScout>(),
		ModelDB.Monster<VermlingShaman>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(1),
		new GainGoldEachReward(30)
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<AshsteelGauntlets>());
	}
}