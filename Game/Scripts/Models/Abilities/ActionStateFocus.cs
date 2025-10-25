using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class ActionState
{
	public const int MuchMoreMoveValue = 30;

	private bool _focusDetermined;
	private Figure _cachedFocus;

	public AIMoveParameters GetAIMoveParameters()
	{
		// These parameters are not cached, due to multiple move abilities being possible in a single action
		// This call is pretty cheap, anyway, so it's fine this way
		AIMoveParameters aiMoveParameters = new AIMoveParameters();

		// Find the earliest following move ability
		for(int i = CurrentAbilityStateIndex; i < Abilities.Count; i++)
		{
			if(Abilities[i] is MoveAbility moveAbility)
			{
				aiMoveParameters.MoveType = moveAbility.MoveType;

				ScenarioCheckEvents.FlyingCheck.Parameters flyingCheckParameters =
					ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(Performer));

				if(flyingCheckParameters.HasFlying)
				{
					aiMoveParameters.MoveType = MoveType.Flying;
				}

				break;
			}
		}

		if(!Performer.HasCondition(Conditions.Disarm))
		{
			// Find the earliest following attack ability
			for(int i = CurrentAbilityStateIndex; i < Abilities.Count; i++)
			{
				if(Abilities[i] is AttackAbility attackAbility)
				{
					aiMoveParameters.Targets = attackAbility.Target.HasFlag(Target.MustTargetSameWithAllTargets) ? 1 : attackAbility.Targets;
					aiMoveParameters.TargetAll = attackAbility.Target.HasFlag(Target.TargetAll);
					aiMoveParameters.Range = attackAbility.Range;
					aiMoveParameters.RangeType = attackAbility.RangeType;
					aiMoveParameters.AOEPattern = attackAbility.AOEPattern;

					break;
				}
			}

			ScenarioCheckEvents.AIMoveParametersCheckEvent.Fire(
				new ScenarioCheckEvents.AIMoveParametersCheck.Parameters(Performer, aiMoveParameters));
		}

		return aiMoveParameters;
	}

	public async GDTask<Figure> GetFocus()
	{
		if(!_focusDetermined || (_cachedFocus != null && _cachedFocus.IsDead))
		{
			_focusDetermined = true;
			_cachedFocus = await DetermineFocus();
		}

		return _cachedFocus;
	}

	// TODO: Change this to a prompt of sorts, to ensure this is saved
	private async GDTask<Figure> DetermineFocus()
	{
		AIMoveParameters aiMoveParameters = GetAIMoveParameters();

		int range = aiMoveParameters.Range; // focusParameters.Range ?? ((Stats.Range ?? 1) + focusParameters.ExtraRange);

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

		List<Hex> rangeCache = new List<Hex>();
		List<FocusNode> bestFocusNodes = new List<FocusNode>();

		// Find all tiles this AI could move to if it had much more movement
		Dictionary<Hex, MoveNode> moreMoveClosedList = new Dictionary<Hex, MoveNode>();
		MoveHelper.FindReachableHexes(
			null, new MoveNode(Performer.Hex, 0, MuchMoreMoveValue, 0),
			Performer, aiMoveParameters.MoveType, moreMoveClosedList, true);

		foreach((Hex moveHex, MoveNode node) in moreMoveClosedList)
		{
			if(!MoveHelper.CanStopAt(Performer, moveHex, aiMoveParameters.MoveType))
			{
				continue;
			}

			rangeCache.Clear();
			RangeHelper.FindHexesInRange(moveHex, range, false, rangeCache);

			void HandlePotentialTargetHex(Hex potentialTargetHex)
			{
				if(potentialTargetHex == null || !map.HasLineOfSight(moveHex, potentialTargetHex))
				{
					return;
				}

				foreach(Figure potentialTarget in potentialTargetHex.GetHexObjectsOfType<Figure>())
				{
					if(!Performer.EnemiesWith(potentialTarget))
					{
						continue;
					}

					ScenarioCheckEvents.CanBeFocusedCheck.Parameters canBeFocusedParameters =
						ScenarioCheckEvents.CanBeFocusedCheckEvent.Fire(new ScenarioCheckEvents.CanBeFocusedCheck.Parameters(Performer, potentialTarget));

					if(!canBeFocusedParameters.CanBeFocused)
					{
						continue;
					}

					ScenarioCheckEvents.PotentialTargetCheck.Parameters potentialTargetCheckParameters =
						ScenarioCheckEvents.PotentialTargetCheckEvent.Fire(
							new ScenarioCheckEvents.PotentialTargetCheck.Parameters(Performer, potentialTarget));

					int adjustedSortingInitiative = potentialTarget.Initiative.SortingInitiative + potentialTargetCheckParameters.SortingInitiativeAdjustment;
					int distanceFromCurrentHex = RangeHelper.Distance(Performer.Hex, potentialTargetHex);
					FocusNode newNode = new FocusNode(potentialTarget, node.NegativeHexEncounteredCount, node.MoveSpent,
						distanceFromCurrentHex, adjustedSortingInitiative, node);
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

		await GDTask.CompletedTask;

		return bestFocusNodes.Count > 0 ? bestFocusNodes[0].Focus : null;
	}
}