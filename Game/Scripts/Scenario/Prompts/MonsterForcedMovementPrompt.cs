using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class MonsterForcedMovementPrompt(
	AbilityState abilityState, Hex origin, Figure target, int distance, ForcedMovementType type,
	EffectCollection effectCollection, Func<string> getHintText, SwingDirectionType? requiredDirection = null)
	: Prompt<MonsterForcedMovementPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public List<Vector2I> Path { get; init; }
	}

	private readonly Dictionary<Hex, ForcedMovementNode> _closedList = new Dictionary<Hex, ForcedMovementNode>();
	private ForcedMovementNode _currentNode;

	private readonly List<ForcedMovementNode> _waypoints = new List<ForcedMovementNode>();
	private readonly List<Hex> _path = new List<Hex>();

	private readonly List<ForcedMovementNode> _bestNodes = new List<ForcedMovementNode>();

	protected override bool CanConfirm => _bestNodes.Any(bestNode => bestNode.Hex == _currentNode.Hex);
	protected override bool CanSkip => false;

	protected override void Enable()
	{
		base.Enable();

		_currentNode = new ForcedMovementNode(target.Hex, 0, distance);
		_waypoints.Clear();
		_waypoints.Add(_currentNode);

		// Find all hexes this AI can push/pull/swing into to
		MoveHelper.FindReachableForcedMovementHexes(abilityState, _currentNode, target, origin, type, _closedList, requiredDirection: requiredDirection);
		_closedList.Add(_currentNode.Hex, _currentNode);

		_bestNodes.Clear();

		foreach((Hex hex, ForcedMovementNode node) in _closedList)
		{
			if(!MoveHelper.CanStopAt(target, hex))
			{
				continue;
			}

			if(_bestNodes.Count == 0)
			{
				_bestNodes.Add(node);
			}
			else
			{
				ForcedMovementNode previousBestNode = _bestNodes[0];
				if(node.MoveSpent > previousBestNode.MoveSpent)
				{
					_bestNodes.Clear();
					_bestNodes.Add(node);
				}

				if(node.MoveSpent == previousBestNode.MoveSpent)
				{
					_bestNodes.Add(node);
				}
			}
		}

		if(_bestNodes.Count == 0 || _bestNodes[0].Hex == _currentNode.Hex)
		{
			// Cannot push/pull further
			Skip();
		}
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		MoveHelper.FindReachableForcedMovementHexes(abilityState, _currentNode, target, origin, type, _closedList, requiredDirection: requiredDirection);

		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		HashSet<Hex> reachableHexes = new HashSet<Hex>();

		// Swing requires recreating possible routes on update
		if(type == ForcedMovementType.Swing) 
		{
			_bestNodes.Clear();
			foreach((Hex hex, ForcedMovementNode node) in _closedList)
			{
				if(!MoveHelper.CanStopAt(target, hex))
				{
					continue;
				}

				_bestNodes.Add(node);
			}
		}

		foreach(ForcedMovementNode bestMoveNode in _bestNodes)
		{
			HandleNode(bestMoveNode);
		}

		bool HandleNode(ForcedMovementNode moveNode)
		{
			bool cameAcrossWaypoint = _waypoints.LastOrDefault()?.Hex == moveNode.Hex;
			foreach(ForcedMovementNode moveNodeParent in moveNode.Parents)
			{
				if(HandleNode(moveNodeParent))
				{
					cameAcrossWaypoint = true;
				}
			}

			if(cameAcrossWaypoint)
			{
				if(moveNode.Hex != _waypoints.LastOrDefault()?.Hex)
				{
					reachableHexes.Add(moveNode.Hex);
				}
			}

			return cameAcrossWaypoint;
		}

		foreach(Hex hex in reachableHexes)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(
				hex, _bestNodes.Any(bestNode => bestNode.Hex == hex) ? HexIndicatorType.Mandatory : HexIndicatorType.Normal, OnIndicatorPressed);
		}

		if(_currentNode != null && _currentNode.Parents.Count > 0)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(_currentNode.Hex, HexIndicatorType.Selected, OnIndicatorPressed);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();

		// Create path out of the waypoints
		_path.Clear();
		ForcedMovementNode pathNode = _currentNode;
		while(pathNode != null)
		{
			_path.Add(pathNode.Hex);
			pathNode = pathNode.Parents.FirstOrDefault();
		}

		_path.Reverse();

		List<Hex> waypointHexes = _waypoints.Select(waypoint => waypoint.Hex).ToList();
		GameController.Instance.MovePath.Open(_path, waypointHexes);
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.HexIndicatorManager.ClearIndicators();
		GameController.Instance.MovePath.Close();
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		if(_currentNode != null && _currentNode.Parents.Count > 0 && _currentNode.Hex == hexIndicator.Hex)
		{
			_waypoints.RemoveAt(_waypoints.Count - 1);
			_currentNode = _waypoints[^1];

			FullUpdateState();
			return;
		}

		if(_closedList.TryGetValue(hexIndicator.Hex, out ForcedMovementNode node))
		{
			_currentNode = node;
			_waypoints.Add(_currentNode);

			FullUpdateState();
		}
	}

	protected override Answer CreateAnswer()
	{
		return new Answer()
		{
			Path = _path.TakeLast(_path.Count - 1).Select(hex => hex.Coords).ToList()
		};
	}
}