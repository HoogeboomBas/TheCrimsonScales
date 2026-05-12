using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario041 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario041.tscn";

	public override int ScenarioNumber => 41;
	public override string Name => "Curse of the Void";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Pursuing the rogue Savvas on the wanted poster, a few coins in the right pocket lets you learn its broad location, and a potted history of it.

		It is a Hollowpact, a Cragheart exile who has made a Faustian pact with Aesthers of the Void who have granted it enormous power, but at the price of its judgement and, in this case, its sanity. All Hollowpacts are inherently unstable, given the Aesthers’ corrupting energy literally bursting out of their bodies, but this one has become dangerous enough to warrant a reward from someone (you suspect the wealthy merchant Councilman Raksani or someone similar has been a victim of the Hollowpact’s crazed destruction).
		
		As night falls in the North of the city near where you were told to go, a purple flash lights up the sky followed by a scream and an explosion. You seem to have found your mark.

		You enter the old building, which is now rocking with explosions from the rear, only to find a small group of bandits. Whether they were disturbed by the Hollowpact, or protecting it, you are not quite sure, but as an arrow passes just overhead, you decide not to wait to find out.
		""";

	public override string ConclusionText =>
		"""
		The Hollowpact does its best to attack you while controlling the increasingly unstable void energy. Eventually it is all too much though—a final strike from you and its body falls, as the void energy explodes from its body in a blinding flash.

		As your eyes readjust, you see that it is very much dead, and set about recovering the many remaining pieces in order to claim the reward. Turns out bounty hunting isn’t always as glamorous as it sounds.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BanditArcher>(),
		ModelDB.Monster<BanditGuard>(),
		ModelDB.Monster<RogueHollowpact>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveGoldReward(50),
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<RogueHollowpact>(), specificCount: 1));

		List<Objective> objectives = GameController.Instance.Map.GetChildrenOfType<Objective>();
		int objectiveHealth = GameController.Instance.SavedCampaign.Characters.Count + GameController.Instance.SavedScenario.ScenarioLevel;
		foreach(Objective objective in objectives)
		{
			objective.Init(objectiveHealth, "Void Pit");
		}
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this,
			canApplyParameters => canApplyParameters.PotentialTarget is Monster monster && monster.MonsterModel is RogueHollowpact &&
			                      !GameController.Instance.Map.Rooms[1].Hexes.Contains(canApplyParameters.Performer.Hex),
			applyParameters =>
			{
				applyParameters.SetCannotBeTargeted();
			});

		ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
			canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is RogueHollowpact &&
			                      !GameController.Instance.Map.Rooms[1].MapTiles.Contains(canApplyParameters.Hex.MapTile),
			applyParameters =>
			{
				applyParameters.SetCanEnter(false);
			}
		);

		await ShowText(
			"""
			Forcing the door, you see the Hollowpact in front of you. It is clearly struggling to control the void energy that is coursing through it, as purple blasts of lightning-like energy are shooting out of his chest cavity and hands, creating swirling pits of dangerous void energy in the floor.

			On seeing you, however, it seems to regain some control of its actions and turns to focus its attention, and power on you.
			""");

		AddScenarioRule(
			$"""
			 The Rogue Hollowpact will not leave the N1B tile. The Rogue Hollowpact cannot be targeted by any figures that are not occupying the N1b tile.
			 """);
	}
}