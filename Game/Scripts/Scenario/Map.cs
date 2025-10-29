using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public partial class Map : Node2D
{
	public const float HexSize = 126f;
	private static readonly float Sqrt3 = Mathf.Sqrt(3);
	public static readonly float HexWidth = Sqrt3 * HexSize;

	public static readonly Vector2I[] NeighbourOffsets =
	{
		new Vector2I(1, -1), // NorthEast
		new Vector2I(1, 0), // East
		new Vector2I(0, 1), // SouthEast
		new Vector2I(-1, 1), // SouthWest
		new Vector2I(-1, 0), // West
		new Vector2I(0, -1), //NorthWest
	};

	public readonly List<MonsterGroup> MonsterGroups = new List<MonsterGroup>();

	public Dictionary<Vector2I, Hex> Hexes { get; } = new Dictionary<Vector2I, Hex>();
	public List<Room> Rooms { get; private set; }

	public List<Door> Doors { get; private set; }

	public List<Treasure> Treasures { get; private set; }

	public List<Marker> Markers { get; private set; }

	public List<Figure> Figures { get; } = new List<Figure>();
	public Figure CurrentTurnTaker { get; private set; }

	public Rect2 ContainerRect { get; private set; }

	public event Action<Rect2> ContainerRectUpdatedEvent;

	public event Action<Figure> FigureAddedEvent;
	public event Action<Figure> FigureRemovedEvent;
	public event Action<Figure> TurnTakerChangedEvent;

	public void Init()
	{
		List<Hex> hexes = this.GetChildrenOfType<Hex>();
		foreach(Hex hex in hexes)
		{
			hex.InitCoords();

			if(Hexes.ContainsKey(hex.Coords))
			{
				// Duplicate hex
				hex.Reparent(this);
				hex.QueueFree();
				continue;
			}

			Hexes.Add(hex.Coords, hex);
		}

		Rooms = this.GetChildrenOfType<Room>();
		foreach(Room room in Rooms)
		{
			room.Init();
		}

		foreach(Hex hex in hexes)
		{
			hex.Reparent(this);
		}

		Doors = this.GetChildrenOfType<Door>();
		foreach(Door door in Doors)
		{
			door.Hide();

			Hex hex = GetHex(GlobalPositionToCoords(door.GlobalPosition), false);

			foreach(Room room in Rooms)
			{
				room.TryAddDoor(door, hex);
			}
		}

		Treasures = this.GetChildrenOfType<Treasure>();

		Markers = this.GetChildrenOfType<Marker>();

		UpdateContainerRect();
	}

	public void RegisterFigure(Figure figure)
	{
		Figures.Add(figure);

		FigureAddedEvent?.Invoke(figure);
	}

	public void DeregisterFigure(Figure figure)
	{
		Figures.Remove(figure);

		FigureRemovedEvent?.Invoke(figure);
	}

	public Hex GetHex(Vector2I coords, bool checkRevealed = true)
	{
		Hex hex = Hexes.GetValueOrDefault(coords);
		return (hex != null && (!checkRevealed || hex.Revealed)) ? hex : null;
	}

	public async GDTask<Monster> CreateMonster(MonsterModel monsterModel, MonsterType monsterType, Vector2I coords, bool summon)
	{
		MonsterGroup monsterGroup = GetMonsterGroup(monsterModel);

		if(monsterType != MonsterType.None && monsterGroup.TryGetAvailableStandeeNumber(out int standeeNumber))
		{
			Hex hex = GetHex(coords);
			Monster monsterHexObject = ResourceLoader.Load<PackedScene>(monsterModel.ScenePath).Instantiate<Monster>();
			AddChild(monsterHexObject, true);
			monsterHexObject.SetMonsterModel(monsterModel);
			await monsterHexObject.Init(hex);
			monsterHexObject.Spawn(monsterGroup, monsterType, standeeNumber, summon);
			return monsterHexObject;
		}

		return null;
	}

	public void UpdateContainerRect()
	{
		float minX = float.MaxValue;
		float maxX = float.MinValue;
		float minY = float.MaxValue;
		float maxY = float.MinValue;
		foreach(KeyValuePair<Vector2I, Hex> coordsTilePair in Hexes)
		{
			Vector2 position = coordsTilePair.Value.GlobalPosition;
			minX = Mathf.Min(minX, position.X);
			maxX = Mathf.Max(maxX, position.X);
			minY = Mathf.Min(minY, position.Y);
			maxY = Mathf.Max(maxY, position.Y);
		}

		ContainerRect = new Rect2(minX, minY, maxX - minX, maxY - minY);

		ContainerRectUpdatedEvent?.Invoke(ContainerRect);
	}

	public void SetTurnTaker(Figure figure)
	{
		CurrentTurnTaker = figure;

		TurnTakerChangedEvent?.Invoke(figure);
	}

	public Marker GetMarker(Marker.Type markerType)
	{
		foreach(Marker marker in Markers)
		{
			if(marker.MarkerType == markerType)
			{
				return marker;
			}
		}

		return null;
	}

	public static Vector2I GetNeighbourCoords(Vector2I coords, int direction)
	{
		return coords + NeighbourOffsets[direction];
	}

	public static Vector2 CoordsToGlobalPosition(Vector2I coords)
	{
		return new Vector2(Sqrt3 * coords.X + Sqrt3 / 2 * coords.Y, 1.5f * coords.Y) * HexSize;
	}

	public static Vector2I GlobalPositionToCoords(Vector2 globalPosition)
	{
		// Algorithm works with specific size, so multiply global point
		globalPosition /= Sqrt3 * HexSize;
		Vector2 point = new Vector2(globalPosition.X, globalPosition.Y);

		int temp = Mathf.FloorToInt(point.X + Sqrt3 * point.Y + 1);
		int r = Mathf.FloorToInt((temp + Mathf.Floor(-point.X + Sqrt3 * point.Y + 1)) / 3);
		int q = Mathf.FloorToInt((Mathf.Floor(2 * point.X + 1) + temp) / 3f) - r;

		return new Vector2I(q, r);
	}

	public static int SimpleDistance(Vector2I coordsA, Vector2I coordsB)
	{
		Vector3I qrsA = QRCoordsToQRS(coordsA);
		Vector3I qrsB = QRCoordsToQRS(coordsB);

		Vector3I vec = qrsA - qrsB; // cube_subtract(a, b)
		return (Mathf.Abs(vec.X) + Mathf.Abs(vec.Y) + Mathf.Abs(vec.Z)) / 2;
		// or: (abs(a.q - b.q) + abs(a.r - b.r) + abs(a.s - b.s)) / 2
	}

	public static Vector2I RotateCoordsClockwise(Vector2I coords, int rotationCount)
	{
		Vector3I tempCoords = QRCoordsToQRS(coords);
		for(int i = 0; i < rotationCount; i++)
		{
			tempCoords = new Vector3I(-tempCoords.Y, -tempCoords.Z, -tempCoords.X);
		}

		return QRSCoordsToQR(tempCoords);
	}

	private static Vector3I QRCoordsToQRS(Vector2I coords)
	{
		return new Vector3I(coords.X, coords.Y, -coords.X - coords.Y);
	}

	private static Vector2I QRSCoordsToQR(Vector3I coords)
	{
		return new Vector2I(coords.X, coords.Y);
	}

	private MonsterGroup GetMonsterGroup(MonsterModel monsterModel)
	{
		MonsterGroup group = MonsterGroups.FirstOrDefault(group => group.MonsterModel == monsterModel);
		if(group == null)
		{
			group = new MonsterGroup(monsterModel, MonsterGroups.Count);
			MonsterGroups.Add(group);
		}

		return group;
	}
}