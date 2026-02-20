using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Map
{
	private const float LineOfSightCenterOffset = 0.95f * HexSize;
	private static readonly Vector2[] LineOfSightOffsets = new[]
	{
		new Vector2(0f, 0f) * LineOfSightCenterOffset, // Center
		new Vector2(0f, -1) * LineOfSightCenterOffset, // North
		new Vector2(0.866f, -0.5f) * LineOfSightCenterOffset, // NorthEast
		new Vector2(0.866f, 0.5f) * LineOfSightCenterOffset, // SouthEast
		new Vector2(0f, 1f) * LineOfSightCenterOffset, // South
		new Vector2(-0.866f, 0.5f) * LineOfSightCenterOffset, // SouthWest
		new Vector2(-0.866f, -0.5f) * LineOfSightCenterOffset, // NorthWest
	};

	private readonly List<WallLine> _wallLines = new List<WallLine>();
	private readonly Dictionary<Hex, List<WallLine>> _hexWallLines = new Dictionary<Hex, List<WallLine>>();

	private readonly Dictionary<Hex, Dictionary<Hex, bool>> _lineOfSightCache = new Dictionary<Hex, Dictionary<Hex, bool>>();
	private readonly Dictionary<Hex, Dictionary<Hex, int>> _distanceCache = new Dictionary<Hex, Dictionary<Hex, int>>();

	private readonly List<WallLine> _relevantWallLineCache = new List<WallLine>();

	public void UpdateWallLines()
	{
		_wallLines.Clear();
		_hexWallLines.Clear();
		_lineOfSightCache.Clear();
		_distanceCache.Clear();

		foreach((Vector2I coords, Hex hex) in Hexes)
		{
			//Hex hex = tilePair.Value;
			for(int i = 0; i < 6; i++)
			{
				Vector2I neighbourCoords = GetNeighbourCoords(coords, i);
				Hex neighbourHex = hex.Neighbours.FirstOrDefault(neighbour => neighbour.Coords == neighbourCoords);

				//if(!Hexes.TryGetValue(neighbourCoords, out Hex neighbourHex) || !neighbourHex.Revealed)
				if(neighbourHex == null || !neighbourHex.Revealed)
				{
					// Create a new wall
					Vector2 centerA = CoordsToGlobalPosition(hex.Coords);
					Vector2 centerB = CoordsToGlobalPosition(neighbourCoords);
					Vector2 lineCenter = (centerA + centerB) * 0.5f;
					Vector2 direction = (centerB - centerA).Normalized();
					direction = direction.Rotated(Mathf.DegToRad(90f));
					//direction = Quaternion.FromEuler(new Vector3(0f, 0f, Mathf.DegToRad(90f))). * direction;
					Vector2 linePointA = lineCenter + direction * 0.5f * HexSize;
					Vector2 linePointB = lineCenter - direction * 0.5f * HexSize;

					WallLine wallLine = new WallLine(linePointA, linePointB);
					_wallLines.Add(wallLine);

					if(!_hexWallLines.TryGetValue(hex, out List<WallLine> hexWallLines))
					{
						hexWallLines = new List<WallLine>();
						_hexWallLines.Add(hex, hexWallLines);
					}

					hexWallLines.Add(wallLine);
				}
			}
		}
	}

	public int? Distance(Hex origin, Hex destination)
	{
		if(origin == null || destination == null)
		{
			return null;
		}

		if(!_distanceCache.TryGetValue(origin, out Dictionary<Hex, int> hexCache))
		{
			hexCache = new Dictionary<Hex, int>();
			_distanceCache.Add(origin, hexCache);
			FloodFillDistance(origin);
		}

		if(!hexCache.TryGetValue(destination, out int distance))
		{
			return null;
		}

		return distance;
	}

	public bool HasLineOfSight(Hex origin, Hex destination)
	{
		if(!_lineOfSightCache.TryGetValue(origin, out Dictionary<Hex, bool> hexCache))
		{
			hexCache = new Dictionary<Hex, bool>();
			_lineOfSightCache.Add(origin, hexCache);
		}

		if(!hexCache.TryGetValue(destination, out bool hasLineOfSight))
		{
			hasLineOfSight = CheckLineOfSight(origin, destination, out _, out _);
			hexCache.Add(destination, hasLineOfSight);
		}

		return hasLineOfSight;
	}

	private void FloodFillDistance(Hex origin)
	{
		List<Node> openList = new List<Node>();
		Dictionary<Hex, Node> closedList = new Dictionary<Hex, Node>();
		// openList.Clear();
		// closedList.Clear();

		Node firstNode = new Node(origin, 0);
		openList.Add(firstNode);
		closedList.Add(firstNode.Hex, firstNode);

		while(openList.Count > 0)
		{
			Node nodeToHandle = openList[0];
			openList.RemoveAt(0);

			foreach(Hex newHex in nodeToHandle.Hex.Neighbours)
			{
				if(newHex != null)
				{
					int rangeCost = 1;
					// int newRangeLeft = nodeToHandle.RangeLeft - rangeCost;
					//
					// if(newRangeLeft < 0)
					// {
					// 	continue;
					// }

					Node newNode = new Node(newHex, nodeToHandle.Distance + rangeCost);

					newNode.Parents.Add(nodeToHandle);

					if(closedList.TryGetValue(newHex, out Node oldNode))
					{
						CompareResult compareResult = newNode.CompareTo(oldNode);
						switch(compareResult)
						{
							case CompareResult.Better:
								// The new node is better than the old one; replace it
								openList.Remove(oldNode);
								openList.Add(newNode);
								closedList[newHex] = newNode;
								break;
							case CompareResult.Worse:
								// The old node is better than the new one; do nothing
								break;
							case CompareResult.Equal:
								// The two nodes are equal in value; keep the old one and add this route as a new potential option
								oldNode.Parents.Add(nodeToHandle);
								break;
						}
					}
					else
					{
						// New node found
						openList.Add(newNode);
						closedList.Add(newHex, newNode);
					}
				}
			}
		}

		foreach((Hex hex, Node node) in closedList)
		{
			_distanceCache[origin][hex] = node.Distance;
		}
	}

	public bool CheckLineOfSight(Hex origin, Hex destination, out Vector2 linePointA, out Vector2 linePointB)
	{
		if(origin == destination)
		{
			linePointA = origin.GlobalPosition;
			linePointB = origin.GlobalPosition;
			return true;
		}

		_relevantWallLineCache.Clear();

		Vector2 originPos = origin.GlobalPosition;
		Vector2 destinationPos = destination.GlobalPosition;

		const float maxDistSquared = (HexSize * 2) * (HexSize * 2) + 1f;
		// Find all hexes with walls that are close enough to possibly influence the LOS
		foreach((Hex hex, List<WallLine> wallLines) in _hexWallLines)
		{
			float distSquared = FindDistanceSquaredToSegment(hex.GlobalPosition, originPos, destinationPos);
			if(distSquared < maxDistSquared)
			{
				_relevantWallLineCache.AddRange(wallLines);
			}
		}

		foreach(Vector2 originLineOfSightOffset in LineOfSightOffsets)
		{
			foreach(Vector2 destinationLineOfSightOffset in LineOfSightOffsets)
			{
				Vector2 pointA = originPos + originLineOfSightOffset;
				Vector2 pointB = destinationPos + destinationLineOfSightOffset;

				bool collision = false;
				foreach(WallLine wallLine in _relevantWallLineCache)
				{
					if(wallLine.Intersects(pointA, pointB))
					{
						collision = true;
						break;
					}
				}

				if(!collision)
				{
					linePointA = pointA;
					linePointB = pointB;
					return true;
				}
			}
		}

		linePointA = default;
		linePointB = default;
		return false;
	}

	// Calculate the distance between
	// point pt and the segment p1 --> p2.
	private static float FindDistanceSquaredToSegment(Vector2 pt, Vector2 p1, Vector2 p2)
	{
		float dx = p2.X - p1.X;
		float dy = p2.Y - p1.Y;

		// Calculate the t that minimizes the distance.
		float t =
			((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
			(dx * dx + dy * dy);

		// See if this represents one of the segment's
		// end points or a point in the middle.
		if(t < 0)
		{
			dx = pt.X - p1.X;
			dy = pt.Y - p1.Y;
		}
		else if(t > 1)
		{
			dx = pt.X - p2.X;
			dy = pt.Y - p2.Y;
		}
		else
		{
			dx = pt.X - (p1.X + t * dx);
			dy = pt.Y - (p1.Y + t * dy);
		}

		return dx * dx + dy * dy;
	}

	public class Node
	{
		public Hex Hex { get; }

		public int Distance { get; }

		public List<Node> Parents { get; } = new List<Node>();

		public Node(Hex hex, int distance)
		{
			Hex = hex;
			Distance = distance;
		}

		public CompareResult CompareTo(Node other)
		{
			if(other.Distance > Distance)
			{
				return CompareResult.Better;
			}

			if(Distance > other.Distance)
			{
				return CompareResult.Worse;
			}

			return CompareResult.Equal;
		}
	}
}