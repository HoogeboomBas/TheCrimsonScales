using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class MovePrompt(MoveAbility.State moveAbilityState, Figure performer, EffectCollection effectCollection, Func<string> getHintText)
	: Prompt<MovePrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public List<Vector2I> Path { get; init; }
		public int MoveSpent { get; init; }
	}

	private readonly Dictionary<Hex, MoveNode> _closedList = new Dictionary<Hex, MoveNode>();
	private MoveNode _currentNode;
	private readonly List<MoveNode> _waypoints = new List<MoveNode>();
	private readonly List<Hex> _path = new List<Hex>();

	protected override bool CanConfirm
	{
		get
		{
			Hex hex = _path.LastOrDefault();
			if(hex == null)
			{
				return false;
			}

			if(!PathExists)
			{
				return false;
			}

			if(_currentNode.MoveLeft == 0)
			{
				return MoveHelper.CanStopAt(moveAbilityState, hex);
			}
			else
			{
				return true;
			}
		}
	}

	protected override bool CanSkip => moveAbilityState.Hexes.Count == 0 || MoveHelper.CanStopAt(moveAbilityState, performer.Hex);

	private bool PathExists => _currentNode != null && _currentNode.Parents.Count > 0;

	protected override void Enable()
	{
		base.Enable();

		_currentNode = new MoveNode(performer.Hex, 0, moveAbilityState.MoveValue, 0);
		_waypoints.Clear();
		_waypoints.Add(_currentNode);
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		MoveHelper.FindReachableHexes(moveAbilityState, _currentNode, performer, moveAbilityState.MoveType, _closedList);

		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		foreach((Hex hex, MoveNode node) in _closedList)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(hex, HexIndicatorType.Normal, OnIndicatorPressed);
		}

		if(_currentNode != null && _currentNode.Parents.Count > 0)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(_currentNode.Hex, HexIndicatorType.Selected, OnIndicatorPressed);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();

		// Create path out of the waypoints
		_path.Clear();
		MoveNode pathNode = _currentNode;
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

		if(_closedList.TryGetValue(hexIndicator.Hex, out MoveNode node))
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
			Path = _path.TakeLast(_path.Count - 1).Select(hex => hex.Coords).ToList(),
			MoveSpent = _currentNode.MoveSpent
		};
	}
}