using System.Collections.Generic;
using Godot;

public static class RangeHelper
{
	private static readonly List<Node> OpenList = new List<Node>();
	private static readonly Dictionary<Hex, Node> ClosedList = new Dictionary<Hex, Node>();

	private static readonly List<Vector2I> OpenCoordsList = new List<Vector2I>();
	private static readonly HashSet<Vector2I> ClosedCoordsList = new HashSet<Vector2I>();

	public static int Distance(Hex origin, Hex destination)
	{
		return GameController.Instance.Map.Distance(origin, destination) ?? int.MaxValue;
	}

	public static void FindHexesInRange(Hex origin, int range, bool requiresLineOfSight, List<Hex> list, bool requiresHexesRevealed = true)
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

				if(newHex.HasHexObjectOfType<Door>()) //newHex != null)
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
			list.Add(nodePair.Key);
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

	public static IEnumerable<Hex> GetHexesInRange(Hex origin, int range, bool includeOrigin = true, bool requiresLineOfSight = true)
	{
		List<Hex> hexes = new List<Hex>();
		FindHexesInRange(origin, range, requiresLineOfSight, hexes);

		foreach(Hex hex in hexes)
		{
			if(!includeOrigin && hex == origin)
			{
				continue;
			}

			yield return hex;
		}
	}

	public static IEnumerable<Figure> GetFiguresInRange(Hex origin, int range, bool includeOrigin = true, bool requiresLineOfSight = true)
	{
		foreach(Hex hex in GetHexesInRange(origin, range, includeOrigin, requiresLineOfSight))
		{
			foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
			{
				yield return figure;
			}
		}
	}

	// public int FindRange(Hex origin, Hex destination, bool requiresLineOfSight)
	// {
	// 	_closedList.Clear();
	// 	_openList.Clear();
	//
	// 	Map map = GameController.Instance.Map;
	//
	// 	Node firstNode = new Node(origin, 0, 1000);
	// 	_openList.Add(firstNode);
	// 	_closedList.Add(firstNode.Hex, firstNode);
	//
	// 	while(_openList.Count > 0)
	// 	{
	// 		Node nodeToHandle = _openList[0];
	// 		_openList.RemoveAt(0);
	//
	// 		if(nodeToHandle.Hex == destination)
	// 		{
	// 			return nodeToHandle.RangeSpent;
	// 		}
	//
	// 		foreach(Point2 neighbourOffset in Map.NeighbourOffsets)
	// 		{
	// 			Point2 newCoords = nodeToHandle.Hex.Coords + neighbourOffset;
	// 			Hex newHex = map.GetHex(newCoords);
	// 			if(newHex != null) // && newHex != firstNode.Hex)
	// 			{
	// 				int rangeCost = 1;
	// 				int newRangeLeft = nodeToHandle.RangeLeft - rangeCost;
	//
	// 				if(newRangeLeft < 0)
	// 				{
	// 					continue;
	// 				}
	//
	// 				Node newNode = new Node(newHex, nodeToHandle.RangeSpent + rangeCost, newRangeLeft);
	//
	// 				newNode.Parents.Add(nodeToHandle);
	//
	// 				if(_closedList.TryGetValue(newHex, out Node oldNode))
	// 				{
	// 					CompareResult compareResult = newNode.CompareTo(oldNode);
	// 					switch(compareResult)
	// 					{
	// 						case CompareResult.Better:
	// 							// The new node is better than the old one; replace it
	// 							_openList.Remove(oldNode);
	// 							_openList.Add(newNode);
	// 							_closedList[newHex] = newNode;
	// 							break;
	// 						case CompareResult.Worse:
	// 							// The old node is better than the new one; do nothing
	// 							break;
	// 						case CompareResult.Equal:
	// 							// The two nodes are equal in value; keep the old one and add this route as a new potential option
	// 							oldNode.Parents.Add(nodeToHandle);
	// 							break;
	// 					}
	// 				}
	// 				else
	// 				{
	// 					if(requiresLineOfSight && !HasLineOfSight(origin, newNode.Hex))
	// 					{
	// 						continue;
	// 					}
	//
	// 					// New node found
	// 					_openList.Add(newNode);
	// 					_closedList.Add(newHex, newNode);
	// 				}
	// 			}
	// 		}
	// 	}
	//
	// 	return int.MaxValue;
	// }

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