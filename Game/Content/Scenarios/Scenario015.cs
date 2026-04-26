// using System.Collections.Generic;
// using Fractural.Tasks;
//
// public class Scenario015 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario015.tscn";
// 	public override int ScenarioNumber => 15;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();
// 	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario016>()];
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new SurviveXRoundsScenarioGoals(10, true);
//
// 	public override List<MonsterModel> MonsterModels { get; } =
// 		[ModelDB.Monster<Hound>(), ModelDB.Monster<CaveBear>(), ModelDB.Monster<RendingDrake>(), ModelDB.Monster<Lurker>()];
//
// 	private readonly List<Marker> _markers = [];
// 	private readonly List<MonsterModel> _monsters =
// 	[
// 		ModelDB.Monster<Hound>(), ModelDB.Monster<CaveBear>(), ModelDB.Monster<GiantViper>(), ModelDB.Monster<RendingDrake>(),
// 		ModelDB.Monster<Lurker>()
// 	];
// 	private int _spawnNumber;
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		UpdateScenarioText($"""
// 		                    At the end of the first round, one island creature spawns at {Icons.Inline(Icons.GetMarker(Marker.Type.a))}
// 		                    At the end of the second round, one island creature spawns at {Icons.Inline(Icons.GetMarker(Marker.Type.c))}
// 		                    At the end of each round after that, two different island creatures will spawn at {Icons.Inline(Icons.GetMarker(Marker.Type.b))} and {Icons.Inline(Icons.GetMarker(Marker.Type.d))} at the end of every odd round, and {Icons.Inline(Icons.GetMarker(Marker.Type.a))} and {Icons.Inline(Icons.GetMarker(Marker.Type.c))} at the end of every even round.
//
// 		                    The type of island creature that spawns cycles in order of Hound, Cave Bear, Giant Viper, Rending Drake, and Lurker. All spawns are normal for two characters. Hounds, Giant Vipers, and Lurkers are elite for three characters. All spawns are elite for four characters.
// 		                    """);
// 		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.a));
// 		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.c));
// 		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.b));
// 		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.d));
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => true,
// 			async parameters =>
// 			{
// 				await SpawnMonster();
// 				if(parameters.RoundNumber > 2)
// 				{
// 					await SpawnMonster();
// 				}
// 			});
// 	}
//
// 	private MonsterType CalculateMonsterType()
// 	{
// 		if(GameController.Instance.SavedCampaign.Characters.Count > 3 ||
// 		   ((_spawnNumber % 5 & 1) == 0 && GameController.Instance.SavedCampaign.Characters.Count == 3))
// 		{
// 			return MonsterType.Elite;
// 		}
//
// 		return MonsterType.Normal;
// 	}
//
// 	private async GDTask SpawnMonster()
// 	{
// 		Hex spawnPoint = _markers[_spawnNumber % 4].Hex;
// 		await SpawnMonster(null, _monsters[_spawnNumber % 5], CalculateMonsterType(), spawnPoint);
// 		_spawnNumber++;
// 	}
// }

