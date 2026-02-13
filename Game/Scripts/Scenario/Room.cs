using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

[GlobalClass]
public partial class Room : Node2D
{
	[Export]
	public bool StartsRevealed { get; private set; }

	private readonly List<(Door, Hex)> _doors = new List<(Door, Hex)>();

	public bool Revealed { get; private set; }

	public List<MapTile> MapTiles { get; private set; }
	public List<Hex> Hexes { get; private set; }

	public List<Figure> Figures => Hexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>()).ToList();

	public void Init()
	{
		// Get all map hexes and parent them to the map, since we want them to be shown to the player at all times
		MapTiles = this.GetChildrenOfType<MapTile>();
		foreach(MapTile mapTile in MapTiles)
		{
			mapTile.Init();
		}

		List<WallHex> wallHexes = this.GetChildrenOfType<WallHex>();
		HashSet<Vector2I> wallCoords = new HashSet<Vector2I>();

		foreach(WallHex wallHex in wallHexes)
		{
			Vector2I coords = Map.GlobalPositionToCoords(wallHex.Position);
			wallCoords.Add(coords);

			wallHex.Init();
		}

		Hexes = this.GetChildrenOfType<Hex>();

		for(int i = Hexes.Count - 1; i >= 0; i--)
		{
			Hex hex = Hexes[i];
			if(wallCoords.Contains(hex.Coords))
			{
				Hexes.RemoveAt(i);
			}
		}

		// Set up neighbours
		foreach(Hex hex in Hexes)
		{
			foreach(Hex otherHex in Hexes)
			{
				if(Map.SimpleDistance(hex.Coords, otherHex.Coords) == 1)
				{
					hex.AddNeighbour(otherHex);
					otherHex.AddNeighbour(hex);
				}
			}
		}

		List<MonsterSpawner> monsterSpawners = this.GetChildrenOfType<MonsterSpawner>();
		foreach(MonsterSpawner monsterSpawner in monsterSpawners)
		{
			GameController.Instance.Map.AddMonsterGroup(ModelDB.GetById<MonsterModel>(new ModelId(monsterSpawner.MonsterModelId)));
		}

		Visible = false;
	}

	public void TryAddDoor(Door door, Hex doorHex)
	{
		// Check if any of this room's tiles neighbours the given door
		bool addedDoor = false;
		foreach(Hex hex in Hexes)
		{
			for(int i = 0; i < 6; i++)
			{
				if(Map.GetNeighbourCoords(hex.Coords, i) == doorHex.Coords)
				{
					if(!addedDoor)
					{
						_doors.Add((door, doorHex));
						door.AddRoom(this);

						addedDoor = true;
					}

					hex.AddNeighbour(doorHex);
					doorHex.AddNeighbour(hex);
				}
			}
		}
	}

	public async GDTask Reveal(Door openedDoor, Figure potentialOpener, bool initializationPhase)
	{
		if(Revealed)
		{
			return;
		}

		Revealed = true;

		Visible = true;

		Map map = GameController.Instance.Map;

		foreach(MapTile mapTile in MapTiles)
		{
			mapTile.Reveal();
		}

		foreach(Hex hex in Hexes)
		{
			hex.Reveal();
		}

		// Initialize each door that hasn't been initialized yet; the first time it's revealed
		foreach((Door otherDoor, Hex hex) in _doors)
		{
			if(!otherDoor.Revealed)
			{
				hex.Reveal();
				int rotationIndex = (Mathf.RoundToInt(otherDoor.GlobalRotationDegrees / 60f) + 6) % 6;
				await otherDoor.Init(hex, rotationIndex);
			}
		}

		List<HexObject> hexObjects = this.GetChildrenOfType<HexObject>();

		foreach(HexObject hexObject in hexObjects)
		{
			Vector2I coords = Map.GlobalPositionToCoords(hexObject.GlobalPosition);
			Hex hex = map.GetHex(coords);
			int rotationIndex = (Mathf.RoundToInt(hexObject.GlobalRotationDegrees / 60f) + 6) % 6;
			await hexObject.Init(hex, rotationIndex);
		}

		List<MonsterSpawner> monsterSpawners = this.GetChildrenOfType<MonsterSpawner>();

		foreach(MonsterSpawner monsterSpawner in monsterSpawners)
		{
			await monsterSpawner.SpawnMonster();
		}

		map.UpdateWallLines();
		map.UpdateContainerRect();

		if(!initializationPhase)
		{
			await ScenarioEvents.RoomRevealedEvent.CreatePrompt(
				new ScenarioEvents.RoomRevealed.Parameters(this, openedDoor, potentialOpener));
		}
	}
}