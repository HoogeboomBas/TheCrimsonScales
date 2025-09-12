using System.Collections.Generic;

public static class MoveHelper
{
	public static void FindReachableHexes(AbilityState abilityState, MoveNode firstNode, Figure performer, MoveType moveType,
		Dictionary<Hex, MoveNode> closedList, bool addFirstNodeToClosedList = false)
	{
		closedList.Clear();

		if(addFirstNodeToClosedList)
		{
			closedList.Add(firstNode.Hex, firstNode);
		}

		// Flood fill to find all reachable hexes
		List<MoveNode> openList = new List<MoveNode>();
		openList.Add(firstNode);
		while(openList.Count > 0)
		{
			MoveNode nodeToHandle = openList[0];
			openList.RemoveAt(0);

			foreach(Hex newHex in nodeToHandle.Hex.Neighbours)
			{
				if(newHex.Revealed && newHex != firstNode.Hex)
				{
					if(!CanPass(abilityState, performer, newHex, false, moveType))
					{
						continue;
					}

					int moveCost = GetMoveCost(performer, newHex, moveType);

					int newNegativeHexEncounteredCount = nodeToHandle.NegativeHexEncounteredCount;
					if(moveType != MoveType.Regular)
					{
						// Make sure to set negative hex count to 0, so jump works appropriately, only triggering any last negative hex
						newNegativeHexEncounteredCount = 0;
					}

					bool affectedByNegativeHex =
						(newHex.HasHexObjectOfType<HazardousTerrain>() || newHex.HasHexObjectOfType<Trap>()) && moveType != MoveType.Flying;

					ScenarioCheckEvents.MoveCheck.Parameters moveCheckParameters =
						ScenarioCheckEvents.MoveCheckEvent.Fire(
							new ScenarioCheckEvents.MoveCheck.Parameters(abilityState, performer, newHex, nodeToHandle.Hex, moveCost, affectedByNegativeHex));

					int newMoveLeft = nodeToHandle.MoveLeft - moveCheckParameters.MoveCost;

					if(newMoveLeft < 0)
					{
						continue;
					}

					if(newMoveLeft == 0 && !CanStopAt(performer, newHex, moveType))
					{
						continue;
					}

					if(moveCheckParameters.AffectedByNegativeHex)
					{
						newNegativeHexEncounteredCount++;
					}

					MoveNode newNode = new MoveNode(newHex, nodeToHandle.MoveSpent + moveCheckParameters.MoveCost, newMoveLeft, newNegativeHexEncounteredCount);

					newNode.Parents.Add(nodeToHandle);

					if(closedList.TryGetValue(newHex, out MoveNode oldNode))
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
	}

	public static void FindReachableFromHexes(AbilityState abilityState, MoveNode firstNode, Figure performer, MoveType moveType,
		Dictionary<Hex, MoveNode> closedList, bool addFirstNodeToClosedList = false)
	{
		closedList.Clear();

		if(!CanStopAt(performer, firstNode.Hex, moveType))
		{
			return;
		}

		if(addFirstNodeToClosedList)
		{
			closedList.Add(firstNode.Hex, firstNode);
		}

		// Flood fill to find all reachable from hexes
		List<MoveNode> openList = new List<MoveNode>();
		openList.Add(firstNode);
		while(openList.Count > 0)
		{
			MoveNode nodeToHandle = openList[0];
			openList.RemoveAt(0);

			Hex fromHex = nodeToHandle.Hex;

			foreach(Hex newHex in nodeToHandle.Hex.Neighbours)
			{
				if(newHex.Revealed && newHex != firstNode.Hex)
				{
					// Check if the old hex can be passed, since we're moving there from the new hex
					if(!CanPass(abilityState, performer, fromHex, false, moveType))
					{
						continue;
					}

					// Moving to the old hex from the new hex, we get the move cost of the old hex
					int moveCost = GetMoveCost(performer, fromHex, moveType);

					int newNegativeHexEncounteredCount = nodeToHandle.NegativeHexEncounteredCount;

					bool affectedByNegativeHex =
						(fromHex.HasHexObjectOfType<HazardousTerrain>() || fromHex.HasHexObjectOfType<Trap>()) && 
						(moveType == MoveType.Regular || (moveType == MoveType.Jump && nodeToHandle == firstNode));

					ScenarioCheckEvents.MoveCheck.Parameters moveCheckParameters =
						ScenarioCheckEvents.MoveCheckEvent.Fire(
							new ScenarioCheckEvents.MoveCheck.Parameters(abilityState, performer, fromHex, newHex, moveCost, affectedByNegativeHex));

					int newMoveLeft = nodeToHandle.MoveLeft - moveCheckParameters.MoveCost;

					if(newMoveLeft < 0)
					{
						continue;
					}

					if(moveCheckParameters.AffectedByNegativeHex)
					{
						newNegativeHexEncounteredCount++;
					}

					MoveNode newNode = new MoveNode(newHex, nodeToHandle.MoveSpent + moveCheckParameters.MoveCost, newMoveLeft, newNegativeHexEncounteredCount);

					newNode.Parents.Add(nodeToHandle);

					if(closedList.TryGetValue(newHex, out MoveNode oldNode))
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
	}

	public static void FindReachablePushPullHexes(AbilityState abilityState, PushPullNode firstNode, Figure target, Hex origin, bool push,
		Dictionary<Hex, PushPullNode> closedList, bool addFirstNodeToClosedList = false)
	{
		closedList.Clear();

		if(addFirstNodeToClosedList)
		{
			closedList.Add(firstNode.Hex, firstNode);
		}

		Map map = GameController.Instance.Map;

		// Flood fill to find all reachable hexes
		List<PushPullNode> openList = new List<PushPullNode>();
		openList.Add(firstNode);
		while(openList.Count > 0)
		{
			PushPullNode nodeToHandle = openList[0];
			openList.RemoveAt(0);

			foreach(Hex newHex in nodeToHandle.Hex.Neighbours)
			{
				if(newHex.Revealed && newHex != firstNode.Hex)
				{
					if(!CanPass(abilityState, target, newHex, true))
					{
						continue;
					}

					int oldDistance = RangeHelper.Distance(nodeToHandle.Hex, origin);
					int newDistance = RangeHelper.Distance(newHex, origin);

					if(push)
					{
						// Pull needs to go away from the performer
						if(oldDistance >= newDistance)
						{
							continue;
						}
					}
					else
					{
						// Pull needs to go towards the performer
						if(oldDistance <= newDistance)
						{
							continue;
						}
					}

					int newMoveLeft = nodeToHandle.MoveLeft - 1;

					if(newMoveLeft < 0)
					{
						continue;
					}

					if(newMoveLeft == 0 && !CanStopAt(target, newHex))
					{
						continue;
					}

					PushPullNode newNode = new PushPullNode(newHex, nodeToHandle.MoveSpent + 1, newMoveLeft);

					newNode.Parents.Add(nodeToHandle);

					if(closedList.TryGetValue(newHex, out PushPullNode oldNode))
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
	}

	public static bool CanPass(AbilityState abilityState, Figure performer, Hex hex, bool forcedMovement, MoveType moveType)
	{
		if(hex.TryGetHexObjectOfType(out Obstacle obstacle) && moveType == MoveType.Regular)
		{
			ScenarioCheckEvents.CanEnterObstacleCheck.Parameters canEnterObstacleParameters =
				ScenarioCheckEvents.CanEnterObstacleCheckEvent.Fire(
					new ScenarioCheckEvents.CanEnterObstacleCheck.Parameters(performer, hex, obstacle, false));

			if(!canEnterObstacleParameters.CanEnter)
			{
				return false;
			}
		}

		if(hex.TryGetHexObjectOfType(out Door door) && (performer is not Character || door.Locked || forcedMovement))
		{
			return false;
		}

		Figure otherFigure = hex.GetHexObjectOfType<Figure>();
		if(otherFigure != null && performer.EnemiesWith(otherFigure) && !otherFigure.HasCondition(Conditions.Invisible) && moveType == MoveType.Regular)
		{
			ScenarioCheckEvents.CanPassEnemyCheck.Parameters canPassEnemyParameters =
				ScenarioCheckEvents.CanPassEnemyCheckEvent.Fire(
					new ScenarioCheckEvents.CanPassEnemyCheck.Parameters(abilityState, performer, otherFigure));

			if(!canPassEnemyParameters.CanPass)
			{
				return false;
			}
		}

		return true;
	}

	public static bool CanPass(AbilityState abilityState, Figure performer, Hex hex, bool forcedMovement)
	{
		return CanPass(abilityState, performer, hex, forcedMovement, MoveType.Regular);
	}

	public static bool CanStopAt(MoveAbility.State moveAbilityState, Hex hex)
	{
		ScenarioCheckEvents.MoveCanStopAtCheck.Parameters parameters =
			ScenarioCheckEvents.MoveCanStopAtCheckEvent.Fire(
				new ScenarioCheckEvents.MoveCanStopAtCheck.Parameters(moveAbilityState, hex));

		if(!parameters.CanStopAt)
		{
			return false;
		}

		return CanStopAt(moveAbilityState.Performer, hex, moveAbilityState.MoveType);
	}

	public static bool CanStopAt(Figure performer, Hex hex, MoveType moveType)
	{
		if(hex.TryGetHexObjectOfType(out Obstacle obstacle) && moveType != MoveType.Flying)
		{
			ScenarioCheckEvents.CanEnterObstacleCheck.Parameters canEnterObstacleParameters =
				ScenarioCheckEvents.CanEnterObstacleCheckEvent.Fire(
					new ScenarioCheckEvents.CanEnterObstacleCheck.Parameters(performer, hex, obstacle, true));

			if(!canEnterObstacleParameters.CanEnter)
			{
				return false;
			}
		}

		Figure otherFigure = hex.GetHexObjectOfType<Figure>();
		if(otherFigure != null && otherFigure != performer)
		{
			return false;
		}

		return true;
	}

	public static bool CanStopAt(Figure performer, Hex hex)
	{
		return CanStopAt(performer, hex, MoveType.Regular);
	}

	public static int GetMoveCost(Figure performer, Hex hex, MoveType moveType)
	{
		if(hex.GetHexObjectOfType<DifficultTerrain>() != null && moveType == MoveType.Regular)
		{
			return 2;
		}

		return 1;
	}
}