using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario058 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario058.tscn";

	public override int ScenarioNumber => 58;
	public override string Name => "Penitentiary Lockdown";
	public override ClassModel ClassModel => ModelDB.Class<ChainguardModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<ChainguardModel>())];

	public override string IntroductionText =>
		"""
		You knew this would happen. Governor Beetleworth was crooked and, worse in Gloomhaven, he was weak. It was an open secret for years that he was on the take; everything could be bought—a cell further away from the sewers, a meal with identifiable meat— but he obviously went too far this time.

		The Assistant, a capable but limited man, found you in the Sleeping Lion, desperate to tell you the story. The short version seems to be that Beetleworth had allowed one of the inmates to take his own exercise—but once he had the key, he slit Beetleworth’s throat and made for the hills, kindly letting the rest of the inmates out before escaping.

		The Assistant (you don’t remember his actual name) senses an opportunity, and is keen to recover the situation, but is incapable of herding the various miscreants back into their cells. So, just like they always do, he winds up at your door—or tavern table in this case. “I just need them back—now!” he pleads. “And whatever you do, don’t kill any of them. I just need things back the way they were.”

		You raise an eyebrow, take a drink, and wait. “Can you do it? And how much?”

		You wait. He knows your price. He just hasn’t realized it yet. “I haven’t got much, but think of the good you’ll be doing to Gloomhaven” the Assistant urges. You smile at that, take another drink, and softly tell him that he knows what you want.

		The Assistant looks confused for a second, then his eyes widen. “That was not approved for use, and should never have been shown to prison employees!” You wait some more. You have time. He doesn’t.

		The Assistant squirms in his seat, wrestling with his conscience, before angrily whispering “Fine! But no-one finds out, no-one gets killed and it gets done now!” You nod in acceptance and drain your drink before getting up. A job’s a job, and it’ll be a good opportunity to catch up with some old acquaintances too.
		""";

	public override string ConclusionText =>
		"Most of the inmates recognized you, and although some tried their luck with you, most of them were too sensible. Order restored, you walk out of the gate, toying with the package the Assistant—presumably soon to be the new Governor—gave you. Job well done.";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BanditArcher>(),
		ModelDB.Monster<BanditGuard>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<ClawTrap>(), ModelDB.Item<ClampTrap>())
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		List<Hex> markersCHexes = GameController.Instance.Map.GetMarkers(Marker.Type.c).Select(marker => marker.Hex).ToList();

		AddScenarioRule("If any enemy dies, the scenario is immediately lost.");
		AddScenarioRule(textParameters => $"Perform {Icons.Inline(Icons.Heal, textParameters)}4, self if you loot a treasure overlay tile.");
		AddScenarioRule(
			"You may forgo a top action while standing adjacent to a door hex while on the F1a tile to permanently close the door and lock a cell. You cannot lock a cell unless there are exactly two enemies inside the room being locked. You cannot lock a cell if an enemy is occupying the door hex of the cell you wish to lock.");
		AddScenarioRule(textParameters =>
			$"Instead of {Icons.Inline(Icons.Attack, textParameters)}2, all basic top actions are {Icons.Inline(Icons.Push, textParameters)}1, {Icons.Inline(Icons.Targets)}1 adjacent enemy, and instead of {Icons.Inline(Icons.Move, textParameters)}2, all basic bottom actions are {Icons.Inline(Icons.Move, textParameters)}2, {Icons.Inline(Icons.Jump, textParameters)}");

		ScenarioRule twoCellsLockedRule = AddScenarioRule("Something will happen when two cells are locked.");

		await AddGoal(new CustomScenarioGoal(_ => "All enemies are locked back in their cells.",
			onStart: async goal =>
			{
				ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
					parameters =>
					{
						if(parameters.ForgoneAction ||
						   parameters.AbilityCardSide.AbilityCardSideType is not AbilityCardSideType.BasicTop and not AbilityCardSideType.Top ||
						   parameters.Performer.Hex.Room != GameController.Instance.Map.Rooms[0])
						{
							return false;
						}

						List<Corridor> corridors = RangeHelper.GetOverlayTilesInRange<Corridor>(parameters.Performer, 1, false).ToList();
						if(corridors.Count != 0 && corridors[0].Hex.IsUnoccupied())
						{
							int index = markersCHexes.IndexOf(corridors[0].Hex);
							if(GameController.Instance.Map.Rooms[index + 1].Figures.Count(figure => parameters.Performer.EnemiesWith(figure)) == 2)
							{
								return true;
							}
						}

						return false;
					},
					async parameters =>
					{
						parameters.ForgoAction();

						Corridor corridor = RangeHelper.GetOverlayTilesInRange<Corridor>(parameters.Performer, 1, false).ToList()[0];
						int index = markersCHexes.IndexOf(corridor.Hex);
						Door door = await AbilityCmd.CreateOverlayTile<Door>(corridor.Hex,
							index < 2
								? SceneLoader.LoadPackedScene("res://Content/OverlayTiles/Doors/StoneDoorHorizontal1H.tscn")
								: SceneLoader.LoadPackedScene("res://Content/OverlayTiles/Doors/StoneDoorVertical1H.tscn"));
						await corridor.Destroy();

						await door.Lock();

						await goal.AdjustProgress(1);
						if(goal.Progress == 2)
						{
							await ShowText(
								"Just as you had them all back, a mocking cry of “Warden!” comes from behind you, as an arrow flies past your ear. You turn slowly and start to swing your chain. You like a challenge.");

							twoCellsLockedRule.Remove();
							ScenarioRule spawnEndRound = new ScenarioRule(textParameters =>
								$"At the end of the round, spawn one elite Bandit Archer in hex {Icons.InlineMarker(Marker.Type.a, textParameters)} one elite Bandit Guard in hex {Icons.InlineMarker(Marker.Type.b, textParameters)}.");

							ScenarioEvents.RoundEndedEvent.Subscribe(this,
								_ => true,
								async _ =>
								{
									await SpawnMonster(null, ModelDB.Monster<BanditArcher>(), MonsterType.Elite,
										GameController.Instance.Map.GetMarker(Marker.Type.a).Hex);
									await SpawnMonster(null, ModelDB.Monster<BanditGuard>(), MonsterType.Elite,
										GameController.Instance.Map.GetMarker(Marker.Type.b).Hex);
									ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
									spawnEndRound.Remove();
								});
						}
					},
					EffectType.Selectable,
					effectButtonParameters: new IconEffectButton.Parameters("res://Art/Icons/Other/Lock.svg"),
					effectInfoViewParameters: new TextEffectInfoView.Parameters("Lock the adjacent cell")
				);

				await GDTask.CompletedTask;
			}, maxProgress: 4));

		foreach(Treasure treasure in GameController.Instance.Map.Treasures)
		{
			treasure.SetObtainLootFunction(async character =>
			{
				await new ActionState(character, [HealAbility.Builder().WithHealValue(4).WithTarget(Target.Self).Build()]).Perform();
			});
		}

		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, AbilityCardSideType.BasicTop,
			parameters => parameters.AbilityCardSide.AbilityCardSideType is AbilityCardSideType.BasicTop,
			async parameters =>
			{
				parameters.SetAbilities(
				[
					PushAbility.Builder()
						.WithPush(1)
						.WithRange(1)
						.Build()
				]);

				await GDTask.CompletedTask;
			});

		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, AbilityCardSideType.BasicBottom,
			parameters => parameters.AbilityCardSide.AbilityCardSideType is AbilityCardSideType.BasicBottom,
			async parameters =>
			{
				parameters.SetAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(2)
						.WithMoveType(MoveType.Jump)
						.Build()
				]);

				await GDTask.CompletedTask;
			});
	}
}