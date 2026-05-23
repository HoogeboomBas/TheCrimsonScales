using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario011 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario011.tscn";

	public override int ScenarioNumber => 11;
	public override string Name => "Voyage Abound";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario012>(true)];

	public override string IntroductionText =>
		"""
		You are in The Crimson Scale, for no reason other than boredom (and a slight disagreement over an outstanding tab at the Sleeping Lion), when Selandre appears, towing a slightly resistant Sankas behind her. “Aha!” she cries in delight, “Just the people I was looking for. You remember my friend Sankas? He requires an artefact from an island in the Misty Sea. Fancy a boat ride?”

		She sits down and continues. “I have arranged your transport ship, and Sankas has full details of what you are looking for.”

		She turns to Sankas, who still seems slightly reluctant as he lays out what he needs.

		“The device I’m building needs a specific power core. They are very old, rare, and extremely powerful. My ancestors were concerned about the damage they could do, so they were scattered and hidden.I only know of one, and as Selandre says, it is kept on a small island offshore. It is not defended as such, but the island is extremely rugged and I would expect numerous wild animals.”

		You look at Selandre with some skeptism. “I thought you liked adventure?” she says. “And yes, you’ll get your reward.” Being firmly land-based, you are not overjoyed about this particular task, but a job’s a job.

		Sure enough, the next day you find a small ship waiting for you with several Inox stationed by the dock. You introduce yourself to them, and without saying a word they lead you onto the ship and prepare to set sail. You are shown to a small cabin each, which you realize are on top of each other, and notice a barrel on the floor in the room, which allows you to communicate and seems to tunnel into the other rooms. Still weary from yesterday’s journey, you decide to take a nap in the small cot on the floor when, just as you’re dozing off, several Inox barge into your room with their weapons drawn.

		“I’m afraid your journey ends here,” one of them snarls.

		You quickly jolt out of bed and grab your weapon, confused and disarrayed. You’re not sure what’s going on here, but you aren’t planning on cutting your journey short now.
		""";

	public override string ConclusionText =>
		"""
		With the demons slain and the Inox dead, you head straight to the captain’s quarters and barge in, only to find them empty. You peer through the captain’s telescope and see a smoky island in the distance. With great effort, (and some luck) you manage to dock the ship safely and gather your belongings, ready to explore the island.

		You are unsure who planned the ambush, or how demons got on your ship, but with treasure on your mind you head onto the island.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<InoxArcher>(),
		ModelDB.Monster<InoxGuard>(),
		ModelDB.Monster<InoxShaman>(),
		ModelDB.Monster<NightDemon>()
	];

	public override List<SavedReward> Rewards =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario012>())
	];

	public override string BGSPath => null;

	private KillAllEnemiesScenarioGoal _goal;

	private ScenarioRule _singleTargetAttackRule;
	private ScenarioRule _cannotLeaveStartingTileRule;
	private ScenarioRule _endOfFirstRoundRule;
	private ScenarioRule _somethingWillHappenRule;

	private List<Obstacle> _barrels;
	private readonly List<Monster> _spawnedMonsters = new List<Monster>();

	private List<Hex> BarrelHexes => _barrels.Select(hex => hex.Hex).ToList();

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new KillAllEnemiesScenarioGoal(enemiesToBeSpawned: true));

		_barrels = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<Obstacle>()).ToList();

		foreach(Room room in GameController.Instance.Map.Rooms)
		{
			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, room,
				canApplyParameters =>
					canApplyParameters.Figure is Character or Summon &&
					canApplyParameters.Hex.MapTile != canApplyParameters.Figure.Hex.MapTile,
				applyParameters =>
				{
					applyParameters.SetCanEnter(false);
				}
			);
		}

		_cannotLeaveStartingTileRule =
			AddScenarioRule("Character and character summons cannot leave their starting tile and the obstacles on the map cannot be destroyed.");
		_singleTargetAttackRule =
			AddScenarioRule(textParameters =>
				$"If a character performs a single-target melee attack while adjacent to an obstacle marked {Icons.InlineMarker(Marker.Type.a, textParameters)}, they may perform the attack as if they were occupying any hex marked {Icons.InlineMarker(Marker.Type.a, textParameters)}, targeting an enemy on another tile.");
		_endOfFirstRoundRule = AddScenarioRule(
			"At the end of the first round, one normal Inox guard and one elite Inox Archer will be spawned on each tile occupied by a character.");
		_somethingWillHappenRule = AddScenarioRule("Something will happen when all the spawned enemies are dead.");

		ScenarioEvents.DuringAttackEvent.Subscribe(this,
			parameters =>
				parameters.Performer is Character && parameters.AbilityState.SingleTargetRangeType == RangeType.Melee &&
				parameters.AbilityState.IsSingleTarget &&
				RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Any(adjacentHex => BarrelHexes.Contains(adjacentHex)),
			async parameters =>
			{
				await parameters.AbilityState.SetPerformHex(hexes =>
				{
					hexes.AddRange(BarrelHexes);
				});
			}, EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters("res://Art/OverlayTiles/Barrel 1h.png"),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Perform the attack as if you were occupying another barrel.")
		);

		List<Hex> hexes = new List<Hex>();
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber == 1,
			async _ =>
			{
				foreach(Character character in GameController.Instance.CharacterManager.Characters)
				{
					hexes.Clear();
					foreach((Vector2I coords, Hex hex) in GameController.Instance.Map.Hexes)
					{
						if(character.Hex.MapTile == hex.MapTile)
						{
							hexes.Add(hex);
						}
					}

					_spawnedMonsters.Add(await SpawnMonster(null, ModelDB.Monster<InoxGuard>(), MonsterType.Normal, hexes));
					_spawnedMonsters.Add(await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Elite, hexes));
				}

				ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

				_endOfFirstRoundRule.Remove();

				ScenarioEvents.FigureKilledEvent.Subscribe(this,
					parameters => _spawnedMonsters.Contains(parameters.Figure),
					async parameters =>
					{
						_spawnedMonsters.Remove(parameters.Figure as Monster);

						if(_spawnedMonsters.Count > 0)
						{
							return;
						}

						await ShowText(
							"You are barely able to catch your breath when more Inox barge in. “You’re not dead?” one of them barks at you while the other starts muttering what sounds like a chant under their breath. No, you are not dead, and as far as you assumed that wasn’t supposed to be part of today’s travel plans.");

						foreach(Character character in GameController.Instance.CharacterManager.Characters)
						{
							hexes.Clear();
							foreach((Vector2I coords, Hex hex) in GameController.Instance.Map.Hexes)
							{
								if(character.Hex.MapTile == hex.MapTile)
								{
									hexes.Add(hex);
								}
							}

							await SpawnMonster(null, ModelDB.Monster<InoxGuard>(), MonsterType.Elite, hexes);
							await SpawnMonster(null, ModelDB.Monster<InoxShaman>(), MonsterType.Elite, hexes);
						}

						ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

						ScenarioEvents.FigureKilledEvent.Subscribe(this,
							_ => GameController.Instance.Map.Figures.All(figure => figure.Alignment != Alignment.Monsters),
							async _ =>
							{
								_somethingWillHappenRule.Remove();

								await ShowText(
									"You peer around the room and listen closely for more footsteps. Silence. You breathe a sigh of relief and as you are about to leave the room to see what’s going on, two demons wisp out from within the barrels and lock eyes. It’s time again.");

								foreach(Character character in GameController.Instance.CharacterManager.Characters)
								{
									hexes.Clear();
									foreach((Vector2I coords, Hex hex) in GameController.Instance.Map.Hexes)
									{
										if(character.Hex.MapTile == hex.MapTile)
										{
											hexes.Add(hex);
										}
									}

									await SpawnMonster(null, ModelDB.Monster<FlameDemon>(), MonsterType.Normal, hexes);
									await SpawnMonster(null, ModelDB.Monster<NightDemon>(), MonsterType.Normal, hexes);
								}

								await _goal.DisableEnemiesToBeSpawned();
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
							}
						);
					}
				);
			}
		);
	}
}