using System;
using System.Collections.Generic;
using Godot;

public class MonsterAOEPrompt(
	AbilityState abilityState, AOEPattern pattern, int range, RangeType rangeType, Figure focus,
	EffectCollection effectCollection, Func<string> getHintText)
	: Prompt<MonsterAOEPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public List<AOEHex> AOEHexes { get; init; }
	}

	private static readonly HashSet<Figure> AttackableFiguresCache = new HashSet<Figure>();

	private readonly List<AIAttackNode> _bestAIAttackNodes = new List<AIAttackNode>();

	private AIAttackNode _selectedNode;

	protected override bool CanSkip => false;

	protected override void Enable()
	{
		base.Enable();

		if(focus == null)
		{
			Skip();
			return;
		}

		bool hasGrayHex = false;

		foreach(AOEHex pivotAOEHex in pattern.LocalHexes)
		{
			if(pivotAOEHex.Type.HasFlag(AOEHexType.Gray))
			{
				hasGrayHex = true;
			}
		}

		Map map = GameController.Instance.Map;

		_bestAIAttackNodes.Clear();

		List<Hex> rangeCache = new List<Hex>();
		RangeHelper.FindHexesInRange(abilityState.Performer.Hex, hasGrayHex ? 0 : range, false, rangeCache);

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

		foreach(Hex hexInRange in rangeCache)
		{
			if(hasGrayHex && hexInRange != abilityState.Performer.Hex)
			{
				continue;
			}

			for(int i = 0; i < 6; i++)
			{
				foreach(AOEHex pivotAOEHex in pattern.LocalHexes)
				{
					if(hasGrayHex && !pivotAOEHex.Type.HasFlag(AOEHexType.Gray))
					{
						continue;
					}

					Figure attackableFocus = null;
					int disadvantageCount = 0;
					AttackableFiguresCache.Clear();

					Vector2I pivotOffset = -pivotAOEHex.Coords;
					foreach(AOEHex aoeHex in pattern.LocalHexes)
					{
						if(!aoeHex.Type.HasFlag(AOEHexType.Red))
						{
							continue;
						}

						Vector2I globalCoords = hexInRange.Coords + Map.RotateCoordsClockwise(pivotOffset + aoeHex.Coords, i);
						Hex potentialTargetHex = map.GetHex(globalCoords);

						if(potentialTargetHex == null || !GameController.Instance.Map.HasLineOfSight(abilityState.Performer.Hex, potentialTargetHex))
						{
							continue;
						}

						foreach(Figure potentialTarget in potentialTargetHex.GetHexObjectsOfType<Figure>())
						{
							if(AttackableFiguresCache.Contains(potentialTarget))
							{
								continue;
							}

							if(!abilityState.Authority.EnemiesWith(potentialTarget))
							{
								continue;
							}

							ScenarioCheckEvents.CanBeTargetedCheck.Parameters canBeTargetedParameters =
								ScenarioCheckEvents.CanBeTargetedCheckEvent.Fire(
									new ScenarioCheckEvents.CanBeTargetedCheck.Parameters(abilityState, abilityState.Performer, potentialTarget));

							if(!canBeTargetedParameters.CanBeTargeted)
							{
								continue;
							}

							// ScenarioCheckEvents.CanBeFocusedCheck.Parameters canBeFocusedParameters =
							// 	ScenarioCheckEvents.CanBeFocusedCheckEvent.Fire(
							// 		new ScenarioCheckEvents.CanBeFocusedCheck.Parameters(performer, potentialTarget));
							//
							// if(!canBeFocusedParameters.CanBeFocused)
							// {
							// 	continue;
							// }

							if(potentialTarget == focus)
							{
								attackableFocus = potentialTarget;
							}

							AttackableFiguresCache.Add(potentialTarget);

							bool rangeDisadvantage =
								AttackAbility.CheckRangeDisadvantage(abilityState.Performer.Hexes, potentialTarget.Hexes, rangeType);
							ScenarioCheckEvents.DisadvantageCheck.Parameters disadvantageCheck =
								ScenarioCheckEvents.DisadvantageCheckEvent.Fire(
									new ScenarioCheckEvents.DisadvantageCheck.Parameters(potentialTarget, abilityState.Performer,
										abilityState.Performer.Hex, rangeDisadvantage));

							if(disadvantageCheck.HasDisadvantage)
							{
								disadvantageCount++;
							}
						}
					}

					// We are ignoring focusParameters.Targets here because it's an AOE. If we have a weird AOE ability like Boldening Blow, that would not work properly.
					int finalTargetCount = AttackableFiguresCache.Count;
					//int finalTargetCount = Mathf.Min(attackableFigureCount, focusParameters.Targets);
					AIAttackNode newAIAttackNode = new AIAttackNode(hexInRange, pivotOffset, i, attackableFocus, finalTargetCount, disadvantageCount);

					CompareAttackNode(newAIAttackNode);
				}
			}
		}

		if(_bestAIAttackNodes.Count == 0)
		{
			// No attacks can be made
			Skip();
			return;
		}

		//TODO: Currently, the player is not allowed to choose the AOE pattern to perform
		_selectedNode = _bestAIAttackNodes[0];
		Complete(true);

		// GameController.Instance.AOEView.AOEChangedEvent += OnAOEChanged;
		// GameController.Instance.AOEView.Open(pattern, forcedOriginHex, abilityState.Performer, range);
	}

	protected override void Disable()
	{
		base.Disable();

		// GameController.Instance.AOEView.AOEChangedEvent -= OnAOEChanged;
		// GameController.Instance.AOEView.Close();
	}

	protected override Answer CreateAnswer()
	{
		List<AOEHex> aoeHexes = [];

		foreach(AOEHex aoeHex in pattern.LocalHexes)
		{
			Vector2I globalCoords =
				_selectedNode.HexInRange.Coords +
				Map.RotateCoordsClockwise(_selectedNode.PivotOffset + aoeHex.Coords, _selectedNode.RotationIndex);
			Hex potentialTargetHex = GameController.Instance.Map.GetHex(globalCoords);

			if(potentialTargetHex == null)
			{
				continue;
			}

			aoeHexes.Add(new AOEHex(globalCoords, aoeHex.Type, aoeHex.CustomMark, aoeHex.IconPath));
		}

		return new Answer()
		{
			AOEHexes = aoeHexes
		};
	}

	private class AIAttackNode
	{
		public Hex HexInRange { get; }
		public Vector2I PivotOffset { get; }
		public int RotationIndex { get; }
		public Figure AttackableFocus { get; }
		public int AttackCount { get; }
		public int DisadvantageCount { get; }

		public AIAttackNode(Hex hexInRange, Vector2I pivotOffset, int rotationIndex, Figure attackableFocus, int attackCount, int disadvantageCount)
		{
			HexInRange = hexInRange;
			PivotOffset = pivotOffset;
			RotationIndex = rotationIndex;
			AttackableFocus = attackableFocus;
			AttackCount = attackCount;
			DisadvantageCount = disadvantageCount;
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

			return CompareResult.Equal;
		}
	}
}