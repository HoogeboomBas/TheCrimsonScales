using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario011 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario011.tscn";
	public override int ScenarioNumber => 11;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario012>(true)];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals(enemiesToBeSpawned: true);

	protected override List<MonsterModel> SpawnedMonsterModels { get; } =
	[
		ModelDB.Monster<CaveBear>(), ModelDB.Monster<FlameDemon>(), ModelDB.Monster<InoxArcher>(), ModelDB.Monster<InoxGuard>(),
		ModelDB.Monster<InoxShaman>(), ModelDB.Monster<NightDemon>()
	];


	public override string BGSPath => null;

	private List<Obstacle> _barrels;
	private List<Hex> BarrelHexes => _barrels.Select(hex => hex.Hex).ToList();

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Muddle);
	}

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		_barrels = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<Obstacle>()).ToList();

		foreach(Room room in GameController.Instance.Map.Rooms)
		{
			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, room,
				canApplyParameters => canApplyParameters.Figure is Character or Summon &&
				                      canApplyParameters.Hex.MapTile != canApplyParameters.Figure.Hex.MapTile,
				applyParameters =>
				{
					applyParameters.SetCanEnter(false);
				}
			);
		}


		UpdateScenarioText("""
		                   At the end of the first round, spawn one normal Inox guard and one elite Inox Archer on each tile occupied by a character.

		                   Something will happen when all the spawned enemies are dead.
		                   """);

		ScenarioEvents.DuringAttackEvent.Subscribe(this,
			parameters => parameters.Performer is Character && parameters.AbilityState.SingleTargetRangeType == RangeType.Melee &&
			              parameters.AbilityState.IsSingleTarget &&
			              RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Intersect(BarrelHexes).Any(),
			async parameters =>
			{
				await parameters.AbilityState.SetPerformHex(hexes =>
				{
					GD.Print(BarrelHexes.Count);
					hexes.AddRange(BarrelHexes);
				});
			}, EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters("res://Art/OverlayTiles/Barrel 1h.png"),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Perform the attack as if you were occupying another barrel"));

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber == 1,
			async _ =>
			{
				foreach(Room room in GameController.Instance.Map.Rooms.Where(room => room.Figures.Any(figure => figure is Character)))
				{
					await SpawnMonster(null, ModelDB.Monster<InoxGuard>(), MonsterType.Normal, room.Hexes);
					await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Elite, room.Hexes);
				}

				UpdateScenarioText("Something will happen when all the spawned enemies are dead.");

				ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

				ScenarioEvents.FigureKilledEvent.Subscribe(this,
					_ => GameController.Instance.Map.Figures.All(figure => figure.Alignment != Alignment.Enemies),
					async _ =>
					{
						foreach(Room room in GameController.Instance.Map.Rooms.Where(room => room.Figures.Any(figure => figure is Character)))
						{
							await SpawnMonster(null, ModelDB.Monster<InoxGuard>(), MonsterType.Elite, room.Hexes);
							await SpawnMonster(null, ModelDB.Monster<InoxShaman>(), MonsterType.Elite, room.Hexes);
						}

						ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

						ScenarioEvents.FigureKilledEvent.Subscribe(this,
							_ => GameController.Instance.Map.Figures.All(figure => figure.Alignment != Alignment.Enemies),
							async _ =>
							{
								UpdateScenarioText("");
								foreach(Room room in GameController.Instance.Map.Rooms.Where(room => room.Figures.Any(figure => figure is Character)))
								{
									await SpawnMonster(null, ModelDB.Monster<FlameDemon>(), MonsterType.Normal, room.Hexes);
									await SpawnMonster(null, ModelDB.Monster<NightDemon>(), MonsterType.Normal, room.Hexes);
								}

								((KillAllEnemiesScenarioGoals)ScenarioGoals).EnemiesToBeSpawned = false;
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
							});
					});
			});
	}

	protected override void UpdateScenarioText(string text)
	{
		string baseText = """
		                  Character and character summons cannot leave their starting tile and the obstacles on the map cannot be destroyed.

		                  If a character performs a single-target melee attack while adjacent to a barrel, they may perform the attack as if they were occupying a hex with a barrel on a different tile, targeting an enemy on the other tile.


		                  """;
		base.UpdateScenarioText(baseText + text);
	}
}