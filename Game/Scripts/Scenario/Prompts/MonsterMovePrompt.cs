using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class MonsterMovePrompt(
	MoveAbility.State moveAbilityState, Figure performer, AIMoveParameters aiMoveParameters, Figure focus, // List<FocusNode> bestFocusNodes,
	EffectCollection effectCollection, Func<string> getHintText)
	: Prompt<MonsterMovePrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public List<Vector2I> Path { get; init; }

		public int MoveSpent { get; init; }
	}

	private readonly Dictionary<Hex, MoveNode> _closedList = new Dictionary<Hex, MoveNode>();
	private readonly Dictionary<Hex, MoveNode> _moreMoveClosedList = new Dictionary<Hex, MoveNode>();
	private readonly Dictionary<Hex, MoveNode> _toBestFocusFromMoveCloserClosedList = new Dictionary<Hex, MoveNode>();
	private MoveNode _currentNode;
	private readonly List<MoveNode> _waypoints = new List<MoveNode>();
	private readonly List<Hex> _path = new List<Hex>();

	private readonly List<AIAttackNode> _bestAIAttackNodes = new List<AIAttackNode>();
	private readonly List<MoveNode> _bestAIMoveNodes = new List<MoveNode>();

	protected override bool CanConfirm => _bestAIMoveNodes.Any(bestAIMoveNode => bestAIMoveNode.Hex == _currentNode.Hex);
	protected override bool CanSkip => false;

	private bool PathExists => _currentNode != null && _currentNode.Parents.Count > 0;

	protected override void Enable()
	{
		base.Enable();

		if(focus == null)
		{
			Skip();
			return;
		}

		_currentNode = new MoveNode(performer.Hex, 0, moveAbilityState.MoveValue, 0);
		_waypoints.Clear();
		_waypoints.Add(_currentNode);

		bool hasGrayHex = false;
		if(aiMoveParameters.AOEPattern != null)
		{
			foreach(AOEHex pivotAOEHex in aiMoveParameters.AOEPattern.Hexes)
			{
				if(pivotAOEHex.Type == AOEHexType.Gray)
				{
					hasGrayHex = true;
				}
			}
		}

		Map map = GameController.Instance.Map;

		// Find all hexes this AI can move to
		MoveHelper.FindReachableHexes(moveAbilityState, _currentNode, performer, moveAbilityState.MoveType, _closedList, true);
		//_closedList.Add(_currentNode.Hex, _currentNode);

		_bestAIAttackNodes.Clear();
		_bestAIMoveNodes.Clear();
		List<Hex> rangeCache = new List<Hex>();

		foreach((Hex moveHex, MoveNode node) in _closedList)
		{
			if(!MoveHelper.CanStopAt(performer, moveHex, aiMoveParameters.MoveType))
			{
				continue;
			}

			rangeCache.Clear();
			RangeHelper.FindHexesInRange(moveHex, aiMoveParameters.Range, true, rangeCache);

			void CompareAttackNode(AIAttackNode newAIAttackNode)
			{
				if(newAIAttackNode.AttackCount == 0)
				{
					return;
				}

				if(_bestAIAttackNodes.Count == 0)
				{
					_bestAIAttackNodes.Add(newAIAttackNode);
				}
				else
				{
					AIAttackNode previousBestAttackNode = _bestAIAttackNodes[0];
					CompareResult compareResult = newAIAttackNode.CompareTo(previousBestAttackNode);
					switch(compareResult)
					{
						case CompareResult.Better:
							_bestAIAttackNodes.Clear();
							_bestAIAttackNodes.Add(newAIAttackNode);
							break;
						case CompareResult.Equal:
							_bestAIAttackNodes.Add(newAIAttackNode);
							break;
						case CompareResult.Worse:
							break;
					}
				}
			}

			//TODO: This can be optimized quite a bit probably
			if(aiMoveParameters.AOEPattern == null)
			{
				Figure attackableFocus = null;
				int attackableFigureCount = 0;
				int disadvantageCount = 0;

				foreach(Hex hexInRange in rangeCache)
				{
					foreach(Figure potentialTarget in hexInRange.GetHexObjectsOfType<Figure>())
					{
						if(!performer.EnemiesWith(potentialTarget))
						{
							continue;
						}

						ScenarioCheckEvents.CanBeTargetedCheck.Parameters canBeTargetedParameters =
							ScenarioCheckEvents.CanBeTargetedCheckEvent.Fire(
								new ScenarioCheckEvents.CanBeTargetedCheck.Parameters(null, performer, potentialTarget));

						if(!canBeTargetedParameters.CanBeTargeted)
						{
							continue;
						}

						ScenarioCheckEvents.CanBeFocusedCheck.Parameters canBeFocusedParameters =
							ScenarioCheckEvents.CanBeFocusedCheckEvent.Fire(new ScenarioCheckEvents.CanBeFocusedCheck.Parameters(performer, potentialTarget));

						if(!canBeFocusedParameters.CanBeFocused)
						{
							continue;
						}

						if(potentialTarget == focus)
						{
							attackableFocus = potentialTarget;
						}

						attackableFigureCount++;

						ScenarioCheckEvents.DisadvantageCheck.Parameters disadvantageCheck = ScenarioCheckEvents.DisadvantageCheckEvent.Fire(
							new ScenarioCheckEvents.DisadvantageCheck.Parameters(potentialTarget, moveAbilityState.Performer, moveHex,
								aiMoveParameters.RangeType == RangeType.Range && RangeHelper.Distance(moveHex, hexInRange) == 1));

						if(disadvantageCheck.HasDisadvantage)
						{
							disadvantageCount++;
						}
					}
				}

				int finalTargetCount = aiMoveParameters.TargetAll
					? attackableFigureCount
					: Mathf.Min(attackableFigureCount, aiMoveParameters.Targets);
				int finalDisadvantageCount = aiMoveParameters.TargetAll ? disadvantageCount : Mathf.Min(disadvantageCount, aiMoveParameters.Targets);
				AIAttackNode newAIAttackNode = new AIAttackNode(node, attackableFocus, finalTargetCount, finalDisadvantageCount, node.MoveSpent);

				CompareAttackNode(newAIAttackNode);
			}
			else // AOE
			{
				foreach(Hex hexInRange in rangeCache)
				{
					if(hasGrayHex && hexInRange != moveHex)
					{
						continue;
					}

					for(int i = 0; i < 6; i++)
					{
						foreach(AOEHex pivotAOEHex in aiMoveParameters.AOEPattern.Hexes)
						{
							if(hasGrayHex && pivotAOEHex.Type != AOEHexType.Gray)
							{
								continue;
							}

							Figure attackableFocus = null;
							int attackableFigureCount = 0;
							int disadvantageCount = 0;

							Vector2I pivotOffset = -pivotAOEHex.LocalCoords;
							foreach(AOEHex aoeHex in aiMoveParameters.AOEPattern.Hexes)
							{
								if(aoeHex.Type != AOEHexType.Red)
								{
									continue;
								}

								Vector2I globalCoords = hexInRange.Coords + Map.RotateCoordsClockwise(pivotOffset + aoeHex.LocalCoords, i);
								Hex potentialTargetHex = map.GetHex(globalCoords);

								if(potentialTargetHex == null || !GameController.Instance.Map.HasLineOfSight(moveHex, potentialTargetHex))
								{
									continue;
								}

								foreach(Figure potentialTarget in potentialTargetHex.GetHexObjectsOfType<Figure>())
								{
									if(!performer.EnemiesWith(potentialTarget))
									{
										continue;
									}

									ScenarioCheckEvents.CanBeTargetedCheck.Parameters canBeTargetedParameters =
										ScenarioCheckEvents.CanBeTargetedCheckEvent.Fire(
											new ScenarioCheckEvents.CanBeTargetedCheck.Parameters(null, performer, potentialTarget));

									if(!canBeTargetedParameters.CanBeTargeted)
									{
										continue;
									}

									ScenarioCheckEvents.CanBeFocusedCheck.Parameters canBeFocusedParameters =
										ScenarioCheckEvents.CanBeFocusedCheckEvent.Fire(new ScenarioCheckEvents.CanBeFocusedCheck.Parameters(performer, potentialTarget));

									if(!canBeFocusedParameters.CanBeFocused)
									{
										continue;
									}

									if(potentialTarget == focus)
									{
										attackableFocus = potentialTarget;
									}

									attackableFigureCount++;

									ScenarioCheckEvents.DisadvantageCheck.Parameters disadvantageCheck =
										ScenarioCheckEvents.DisadvantageCheckEvent.Fire(
											new ScenarioCheckEvents.DisadvantageCheck.Parameters(potentialTarget, moveAbilityState.Performer, moveHex,
												aiMoveParameters.RangeType == RangeType.Range &&
												RangeHelper.Distance(moveHex, potentialTargetHex) == 1));

									if(disadvantageCheck.HasDisadvantage)
									{
										disadvantageCount++;
									}
								}
								//HandlePotentialTargetHex(potentialTargetHex);
							}

							// We are ignoring focusParameters.Targets here because it's an AOE. If we have a weird AOE ability like Boldening Blow, that would not work properly.
							int finalTargetCount = attackableFigureCount;
							//int finalTargetCount = Mathf.Min(attackableFigureCount, focusParameters.Targets);
							//int finalDisadvantageCount = Mathf.Min(disadvantageCount, focusParameters.Targets);
							AIAttackNode newAIAttackNode =
								new AIAttackNode(node, attackableFocus, finalTargetCount, disadvantageCount, node.MoveSpent);

							CompareAttackNode(newAIAttackNode);
						}
					}
				}
			}
		}

		_bestAIMoveNodes.AddRange(_bestAIAttackNodes.Select(attackNode => attackNode.Node));

		// FOCUS NODE CALCULATION
		List<FocusNode> bestFocusNodes = new List<FocusNode>();

		int range = aiMoveParameters.Range;

		// Find all hexes this AI could move to if it had much more movement
		MoveHelper.FindReachableHexes(
			null, new MoveNode(performer.Hex, 0, ActionState.MuchMoreMoveValue, 0),
			performer, aiMoveParameters.MoveType, _moreMoveClosedList, true);

		bestFocusNodes.Clear();
		foreach((Hex moveHex, MoveNode node) in _moreMoveClosedList)
		{
			if(!MoveHelper.CanStopAt(performer, moveHex, moveAbilityState.MoveType))
			{
				continue;
			}

			rangeCache.Clear();
			RangeHelper.FindHexesInRange(moveHex, range, false, rangeCache);

			void HandlePotentialTargetHex(Hex potentialTargetHex)
			{
				if(potentialTargetHex != focus.Hex)
				{
					return;
				}

				if(potentialTargetHex == null || !map.HasLineOfSight(moveHex, potentialTargetHex))
				{
					return;
				}

				foreach(Figure potentialTarget in potentialTargetHex.GetHexObjectsOfType<Figure>())
				{
					if(potentialTarget != focus)
					{
						continue;
					}

					// if(!performer.EnemiesWith(potentialTarget))
					// {
					// 	continue;
					// }

					int distanceFromCurrentHex = RangeHelper.Distance(performer.Hex, potentialTargetHex);
					FocusNode newNode = new FocusNode(potentialTarget, node.NegativeHexEncounteredCount, node.MoveSpent,
						distanceFromCurrentHex, potentialTarget.Initiative.SortingInitiative, node);
					if(bestFocusNodes.Count == 0)
					{
						bestFocusNodes.Add(newNode);
					}
					else
					{
						FocusNode previousBestNode = bestFocusNodes[0];
						CompareResult compareResult = newNode.CompareTo(previousBestNode);
						switch(compareResult)
						{
							case CompareResult.Better:
								bestFocusNodes.Clear();
								bestFocusNodes.Add(newNode);
								break;
							case CompareResult.Equal:
								bestFocusNodes.Add(newNode);
								break;
							case CompareResult.Worse:
								break;
						}
					}
				}
			}

			//TODO: This can be optimized quite a bit probably
			foreach(Hex hexInRange in rangeCache)
			{
				if(aiMoveParameters.AOEPattern == null)
				{
					HandlePotentialTargetHex(hexInRange);
					continue;
				}

				if(hasGrayHex && hexInRange != moveHex)
				{
					continue;
				}

				for(int i = 0; i < 6; i++)
				{
					foreach(AOEHex pivotAOEHex in aiMoveParameters.AOEPattern.Hexes)
					{
						if(hasGrayHex && pivotAOEHex.Type != AOEHexType.Gray)
						{
							continue;
						}

						Vector2I pivotOffset = -pivotAOEHex.LocalCoords;
						foreach(AOEHex aoeHex in aiMoveParameters.AOEPattern.Hexes)
						{
							if(aoeHex.Type != AOEHexType.Red)
							{
								continue;
							}

							Vector2I globalCoords = hexInRange.Coords + Map.RotateCoordsClockwise(pivotOffset + aoeHex.LocalCoords, i);
							Hex potentialTargetHex = map.GetHex(globalCoords);

							HandlePotentialTargetHex(potentialTargetHex);
						}
					}
				}
			}
		}
		// END FOCUS NODE CALCULATION

		// If no attack can be performed with the current move distance, or there is a better path available with fewer negative hexes, follow another path
		if(_bestAIAttackNodes.Count == 0 || (bestFocusNodes[0].NegativeHexEncounteredCount < _bestAIAttackNodes[0].NegativeHexEncounteredCount))
		{
			_bestAIMoveNodes.Clear();

			// Cannot attack anything this round, time to move closer to the hex it can attack focus from
			// Move toward (more specifically; move closer to) one of the `bestFocusNodes`

			List<MoveCloserNode> moveCloserNodes = new List<MoveCloserNode>();

			foreach(FocusNode focusNode in bestFocusNodes)
			{
				// Find all paths that lead to this focus node
				MoveHelper.FindReachableFromHexes(
					null, new MoveNode(focusNode.MoveNode.Hex, 0, ActionState.MuchMoreMoveValue, 0),
					performer, aiMoveParameters.MoveType, _toBestFocusFromMoveCloserClosedList, true);

				foreach((Hex hex, MoveNode moveNode) in _closedList)
				{
					if(!MoveHelper.CanStopAt(performer, hex, moveAbilityState.MoveType))
					{
						continue;
					}

					// Check if the focus node has a parent going down to this node within move range
					foreach((Hex toBestFocusFromMoveCloserNodeHex, MoveNode toBestFocusFromMoveCloserNode) in _toBestFocusFromMoveCloserClosedList)
					{
						if(toBestFocusFromMoveCloserNodeHex == hex)
						{
							// If the path with this stop results in more negative hexes, just don't bother
							int totalNegativeHexCount =
								moveNode.NegativeHexEncounteredCount + toBestFocusFromMoveCloserNode.NegativeHexEncounteredCount;
							if(totalNegativeHexCount > focusNode.NegativeHexEncounteredCount)
							{
								continue;
							}

							MoveCloserNode newMoveCloserNode = new MoveCloserNode(moveNode, toBestFocusFromMoveCloserNode);

							if(moveCloserNodes.Count == 0)
							{
								moveCloserNodes.Add(newMoveCloserNode);
							}
							else
							{
								MoveCloserNode previousBestMoveCloserNode = moveCloserNodes[0];
								CompareResult compareResult =
									newMoveCloserNode.CompareTo(previousBestMoveCloserNode, moveAbilityState.MoveType == MoveType.Jump);
								switch(compareResult)
								{
									case CompareResult.Better:
										moveCloserNodes.Clear();
										moveCloserNodes.Add(newMoveCloserNode);
										break;
									case CompareResult.Equal:
										moveCloserNodes.Add(newMoveCloserNode);
										break;
									case CompareResult.Worse:
										break;
								}
							}
						}
					}
				}
			}

			_bestAIMoveNodes.AddRange(moveCloserNodes.Select(moveCloserNode => moveCloserNode.MoveNode));
		}

		if(_bestAIMoveNodes.Count == 0 || _bestAIMoveNodes[0].Hex == _currentNode.Hex)
		{
			// This figure is already at the optimal position
			Skip();
			return;
		}

		if(_bestAIMoveNodes.All(bestAIMoveNode => bestAIMoveNode.Hex == _bestAIMoveNodes[0].Hex))
		{
			// There is only one real option for movement available
			//TODO: Implement check event to see if movement matters for any character or effect (for certain hexes), like walking over rifts/shadows being
			//TODO: beneficial for the player

			_currentNode = _bestAIMoveNodes[0];


			// Create path out of the waypoints
			CreatePath();
			// // Create path out of the waypoints
			// _path.Clear();
			// MoveNode pathNode = _currentNode;
			// while(pathNode != null)
			// {
			// 	_path.Add(pathNode.Hex);
			// 	pathNode = pathNode.Parents.FirstOrDefault();
			// }
			//
			// _path.Reverse();

			Complete(true);
		}

		AppController.Instance.InputController.SkipAIDecisionEvent += OnSkipAIDecision;
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		MoveHelper.FindReachableHexes(moveAbilityState, _currentNode, performer, moveAbilityState.MoveType, _closedList);

		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		HashSet<Hex> reachableHexes = new HashSet<Hex>();

		foreach(MoveNode bestMoveNode in _bestAIMoveNodes)
		{
			HandleNode(bestMoveNode);
		}

		bool HandleNode(MoveNode moveNode)
		{
			bool cameAcrossWaypoint = _waypoints.LastOrDefault()?.Hex == moveNode.Hex;
			foreach(MoveNode moveNodeParent in moveNode.Parents)
			{
				if(HandleNode(moveNodeParent))
				{
					cameAcrossWaypoint = true;
				}
			}

			if(cameAcrossWaypoint)
			{
				if(moveNode.Hex == _waypoints.LastOrDefault()?.Hex)
				{
					//waypointHexes.AddIfNew(moveNode.Hex);
				}
				else
				{
					reachableHexes.Add(moveNode.Hex);
				}
			}

			return cameAcrossWaypoint;
		}

		foreach(Hex hex in reachableHexes)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(
				hex, _bestAIMoveNodes.Any(bestAIMoveNode => bestAIMoveNode.Hex == hex) ? HexIndicatorType.Mandatory : HexIndicatorType.Normal,
				OnIndicatorPressed);
		}

		if(_currentNode != null && _currentNode.Parents.Count > 0)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(_currentNode.Hex, HexIndicatorType.Selected, OnIndicatorPressed);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();

		// Create path out of the waypoints
		CreatePath();
		// // Create path out of the waypoints
		// _path.Clear();
		// MoveNode pathNode = _currentNode;
		// while(pathNode != null)
		// {
		// 	_path.Add(pathNode.Hex);
		// 	pathNode = pathNode.Parents.FirstOrDefault();
		// }
		//
		// _path.Reverse();

		List<Hex> waypointHexes = _waypoints.Select(waypoint => waypoint.Hex).ToList();
		GameController.Instance.MovePath.Open(_path, waypointHexes);
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.HexIndicatorManager.ClearIndicators();
		GameController.Instance.MovePath.Close();

		AppController.Instance.InputController.SkipAIDecisionEvent -= OnSkipAIDecision;
	}

	private void CreatePath()
	{
		_path.Clear();
		MoveNode pathNode = _currentNode;
		while(pathNode != null)
		{
			_path.Add(pathNode.Hex);
			pathNode = pathNode.Parents.FirstOrDefault();
		}

		_path.Reverse();
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

	private void OnSkipAIDecision()
	{
		_currentNode = _bestAIMoveNodes.PickRandom(GameController.Instance.VisualRNG);

		// Create path out of the waypoints
		CreatePath();

		Complete();
	}

	protected override Answer CreateAnswer()
	{
		return new Answer()
		{
			Path = _path.TakeLast(_path.Count - 1).Select(hex => hex.Coords).ToList(),
			MoveSpent = _currentNode.MoveSpent,
			//CanAttack = _canAttack
		};
	}

	private class AIAttackNode
	{
		public MoveNode Node { get; }
		public Figure AttackableFocus { get; }
		public int AttackCount { get; }
		public int DisadvantageCount { get; }
		public int MoveSpent { get; }

		public int NegativeHexEncounteredCount => Node.NegativeHexEncounteredCount;

		public AIAttackNode(MoveNode node, Figure attackableFocus, int attackCount, int disadvantageCount, int moveSpent)
		{
			Node = node;
			AttackableFocus = attackableFocus;
			AttackCount = attackCount;
			DisadvantageCount = disadvantageCount;
			MoveSpent = moveSpent;
		}

		public CompareResult CompareTo(AIAttackNode other)
		{
			if(AttackableFocus != null && other.AttackableFocus == null)
			{
				return CompareResult.Better;
			}

			if(other.AttackableFocus != null && AttackableFocus == null)
			{
				return CompareResult.Worse;
			}

			if(NegativeHexEncounteredCount > other.NegativeHexEncounteredCount)
			{
				return CompareResult.Worse;
			}

			if(other.NegativeHexEncounteredCount > NegativeHexEncounteredCount)
			{
				return CompareResult.Better;
			}

			if(AttackCount > other.AttackCount)
			{
				return CompareResult.Better;
			}

			if(other.AttackCount > AttackCount)
			{
				return CompareResult.Worse;
			}

			if(DisadvantageCount > other.DisadvantageCount)
			{
				return CompareResult.Worse;
			}

			if(other.DisadvantageCount > DisadvantageCount)
			{
				return CompareResult.Better;
			}

			if(MoveSpent > other.MoveSpent)
			{
				return CompareResult.Worse;
			}

			if(other.MoveSpent > MoveSpent)
			{
				return CompareResult.Better;
			}

			return CompareResult.Equal;
		}
	}

	private class MoveCloserNode
	{
		public MoveNode MoveNode { get; }
		public MoveNode ToBestFocusFromMoveCloserNode { get; }

		public int MoveSpent => MoveNode.MoveSpent;
		//public int NegativeHexEncounteredCount => MoveNode.NegativeHexEncounteredCount;

		public int ToDestinationMoveRequired => ToBestFocusFromMoveCloserNode.MoveSpent;

		public MoveCloserNode(MoveNode moveNode, MoveNode toBestFocusFromMoveCloserNode)
		{
			MoveNode = moveNode;
			ToBestFocusFromMoveCloserNode = toBestFocusFromMoveCloserNode;
		}

		public CompareResult CompareTo(MoveCloserNode other, bool jumping)
		{
			//TODO: The commented out code can probably be removed, but I'm not 100% sure hmmm...
			// if(jumping)
			// {
			// 	// When jumping, if the final hex of a move closer node is negative, we'd rather not jump into it
			// 	if(NegativeHexEncounteredCount > other.NegativeHexEncounteredCount)
			// 	{
			// 		return CompareResult.Worse;
			// 	}
			//
			// 	if(other.NegativeHexEncounteredCount > NegativeHexEncounteredCount)
			// 	{
			// 		return CompareResult.Better;
			// 	}
			// }

			if(ToDestinationMoveRequired > other.ToDestinationMoveRequired)
			{
				return CompareResult.Worse;
			}

			if(other.ToDestinationMoveRequired > ToDestinationMoveRequired)
			{
				return CompareResult.Better;
			}

			if(MoveSpent > other.MoveSpent)
			{
				return CompareResult.Worse;
			}

			if(other.MoveSpent > MoveSpent)
			{
				return CompareResult.Better;
			}

			return CompareResult.Equal;
		}
	}
}