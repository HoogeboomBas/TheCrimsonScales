using System.Collections.Generic;
using Godot;

public static class RangeHelper
{
	public const int InfiniteRange = 100;

	private static readonly List<Node> OpenList = new List<Node>();
	private static readonly Dictionary<Hex, Node> ClosedList = new Dictionary<Hex, Node>();

	private static readonly List<Vector2I> OpenCoordsList = new List<Vector2I>();
	private static readonly HashSet<Vector2I> ClosedCoordsList = new HashSet<Vector2I>();

	public static int Distance(Hex origin, Hex destination)
	{
		return GameController.Instance.Map.Distance(origin, destination) ?? InfiniteRange;
	}

	public static void FindHexesInRange(Hex origin, int range, bool requiresLineOfSight, List<Hex> list,
		bool requiresHexesRevealed = true, bool allowDoors = false, int minRange = 0)
	{
		OpenList.Clear();
		ClosedList.Clear();

		Map map = GameController.Instance.Map;

		Node firstNode = new Node(origin, 0, range);
		OpenList.Add(firstNode);
		ClosedList.Add(firstNode.Hex, firstNode);

		while(OpenList.Count > 0)
		{
			Node nodeToHandle = OpenList[0];
			OpenList.RemoveAt(0);

			foreach(Hex newHex in nodeToHandle.Hex.Neighbours)
			{
				if(requiresHexesRevealed && !newHex.Revealed)
				{
					continue;
				}

				if(!allowDoors && newHex.HasHexObjectOfType<Door>()) //newHex != null)
				{
					continue;
				}

				int rangeCost = 1;
				int newRangeLeft = nodeToHandle.RangeLeft - rangeCost;

				if(newRangeLeft < 0)
				{
					continue;
				}

				Node newNode = new Node(newHex, nodeToHandle.RangeSpent + rangeCost, newRangeLeft);

				newNode.Parents.Add(nodeToHandle);

				if(ClosedList.TryGetValue(newHex, out Node oldNode))
				{
					CompareResult compareResult = newNode.CompareTo(oldNode);
					switch(compareResult)
					{
						case CompareResult.Better:
							// The new node is better than the old one; replace it
							OpenList.Remove(oldNode);
							OpenList.Add(newNode);
							ClosedList[newHex] = newNode;
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
					if(requiresLineOfSight && !map.HasLineOfSight(origin, newNode.Hex))
					{
						continue;
					}

					// New node found
					OpenList.Add(newNode);
					ClosedList.Add(newHex, newNode);
				}
			}
		}

		foreach(KeyValuePair<Hex, Node> nodePair in ClosedList)
		{
			if(nodePair.Value.RangeSpent >= minRange)
			{
				list.Add(nodePair.Key);
			}
		}
	}

	public static void FindCoordsInRange(Vector2I origin, int range, List<Vector2I> list)
	{
		OpenCoordsList.Clear();
		ClosedCoordsList.Clear();

		OpenCoordsList.Add(origin);
		ClosedCoordsList.Add(origin);

		while(OpenCoordsList.Count > 0)
		{
			Vector2I coordsToHandle = OpenCoordsList[0];
			OpenCoordsList.RemoveAt(0);

			for(int i = 0; i < 6; i++)
			{
				Vector2I newCoords = coordsToHandle.Add((Direction)i);

				int totalRange = Map.SimpleDistance(origin, newCoords);

				if(totalRange <= range && !ClosedCoordsList.Contains(newCoords))
				{
					// New node found
					if(totalRange < range)
					{
						OpenCoordsList.Add(newCoords);
					}

					ClosedCoordsList.Add(newCoords);
				}
			}
		}

		list.AddRange(ClosedCoordsList);
	}

	public static IEnumerable<Hex> GetHexesInRange(Hex origin, int range, bool includeOrigin = true, bool requiresLineOfSight = true,
		bool requiresHexesRevealed = true, bool allowDoors = false)
	{
		List<Hex> hexes = new List<Hex>();
		FindHexesInRange(origin, range, requiresLineOfSight, hexes, requiresHexesRevealed, allowDoors);

		foreach(Hex hex in hexes)
		{
			if(!includeOrigin && hex == origin)
			{
				continue;
			}

			yield return hex;
		}
	}

	public static IEnumerable<Figure> GetFiguresInRange(Hex origin, int range, bool includeOrigin = true, bool requiresLineOfSight = true,
		bool includeNonFigures = false)
	{
		foreach(Hex hex in GetHexesInRange(origin, range, includeOrigin, requiresLineOfSight))
		{
			foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
			{
				if(includeNonFigures || figure.IsFigure)
				{
					yield return figure;
				}
			}
		}
	}

	public static IEnumerable<Figure> GetFiguresInRange(HexObject hexObject, int range, bool includeOrigin = true, bool requiresLineOfSight = true,
		bool includeNonFigures = false)
	{
		List<Figure> figures = new List<Figure>();

		foreach(Hex objectHex in hexObject.Hexes)
		{
			foreach(Figure figure in GetFiguresInRange(objectHex, range, includeOrigin, requiresLineOfSight, includeNonFigures: includeNonFigures))
			{
				figures.AddIfNew(figure);
			}
		}

		return figures;
	}

	public static bool CheckCanPlaceObstacle(Hex targetHex)
	{
		// Find all neighbours of the targetHex, and check if one of its neighbours is still connected to the other neighbors
		List<Hex> walkableNeighbours = new List<Hex>();
		foreach(Hex neighbour in targetHex.Neighbours)
		{
			if(neighbour.HasHexObjectOfType<Obstacle>() || neighbour.HasHexObjectOfType<Door>())
			{
				continue;
			}

			walkableNeighbours.Add(neighbour);
		}

		if(walkableNeighbours.Count < 2)
		{
			return true;
		}

		OpenList.Clear();
		ClosedList.Clear();

		Node firstNode = new Node(walkableNeighbours[0], 0, 1000);
		OpenList.Add(firstNode);
		ClosedList.Add(firstNode.Hex, firstNode);

		while(OpenList.Count > 0)
		{
			Node nodeToHandle = OpenList[0];
			OpenList.RemoveAt(0);

			foreach(Hex newHex in nodeToHandle.Hex.Neighbours)
			{
				if(!newHex.Revealed)
				{
					continue;
				}

				if(newHex.HasHexObjectOfType<Door>()) //newHex != null)
				{
					continue;
				}

				if(newHex.HasHexObjectOfType<Obstacle>() || newHex == targetHex)
				{
					continue;
				}

				int rangeCost = 1;
				int newRangeLeft = nodeToHandle.RangeLeft - rangeCost;

				if(newRangeLeft < 0)
				{
					continue;
				}

				Node newNode = new Node(newHex, nodeToHandle.RangeSpent + rangeCost, newRangeLeft);

				newNode.Parents.Add(nodeToHandle);

				if(ClosedList.TryGetValue(newHex, out Node oldNode))
				{
					CompareResult compareResult = newNode.CompareTo(oldNode);
					switch(compareResult)
					{
						case CompareResult.Better:
							// The new node is better than the old one; replace it
							OpenList.Remove(oldNode);
							OpenList.Add(newNode);
							ClosedList[newHex] = newNode;
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
					OpenList.Add(newNode);
					ClosedList.Add(newHex, newNode);
				}
			}
		}

		foreach(Hex walkableNeighbour in walkableNeighbours)
		{
			if(!ClosedList.ContainsKey(walkableNeighbour))
			{
				return false;
			}
		}

		return true;
	}

	public class Node
	{
		public Hex Hex { get; }

		public int RangeSpent { get; }

		public int RangeLeft { get; }

		public List<Node> Parents { get; } = new List<Node>();

		public Node(Hex hex, int rangeSpent, int rangeLeft)
		{
			Hex = hex;
			RangeSpent = rangeSpent;
			RangeLeft = rangeLeft;
		}

		public CompareResult CompareTo(Node other)
		{
			if(other.RangeSpent > RangeSpent)
			{
				return CompareResult.Better;
			}

			if(RangeSpent > other.RangeSpent)
			{
				return CompareResult.Worse;
			}

			return CompareResult.Equal;
		}
	}
}