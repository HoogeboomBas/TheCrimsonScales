using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweensGodot.Extensions;

public class SingleTargetState
{
	public Figure Target { get; init; }
	public List<Hex> ForcedMovementHexes { get; } = new List<Hex>();
}

public abstract class TargetedAbilityState<TSingleTargetState> : TargetedAbilityState
	where TSingleTargetState : SingleTargetState, new()
{
	public List<TSingleTargetState> SingleTargetStates { get; } = new List<TSingleTargetState>();

	public TSingleTargetState SingleTargetState { get; set; }

	public override Figure Target => SingleTargetState.Target;

	public void AddSingleTargetState(Figure target)
	{
		SingleTargetState = new TSingleTargetState
		{
			Target = target
		};

		SingleTargetStates.Add(SingleTargetState);
	}
}

public abstract class TargetedAbilityState : AbilityState
{
	public List<Figure> UniqueTargetedFigures { get; } = new List<Figure>();
	public List<Hex> TargetedHexes { get; } = new List<Hex>();
	public Dictionary<Vector2I, AOEHexType> AOEHexes { get; set; }

	public Target AbilityTarget { get; set; }
	public int AbilityTargets { get; set; }
	public AOEPattern AbilityAOEPattern { get; set; }

	public RangeType AbilityRangeType { get; set; }
	public int AbilityRange { get; set; }
	public List<ConditionModel> AbilityConditionModels { get; set; }
	public int AbilityPush { get; set; }
	public int AbilityPull { get; set; }
	public int AbilitySwing { get; set; }


	public RangeType SingleTargetRangeType { get; set; }
	public int SingleTargetRange { get; set; }
	public List<ConditionModel> SingleTargetConditionModels { get; set; }
	public int SingleTargetPush { get; set; }
	public int SingleTargetPull { get; set; }
	public int SingleTargetSwing { get; set; }

	public abstract Figure Target { get; }

	public IEnumerable<Hex> GetRedAOEHexes()
	{
		if(AOEHexes == null)
		{
			yield break;
		}

		foreach((Vector2I coords, AOEHexType type) in AOEHexes)
		{
			Hex hex = GameController.Instance.Map.GetHex(coords);

			if(hex != null && type == AOEHexType.Red)
			{
				yield return hex;
			}
		}
	}

	public void AdjustTarget(Target target)
	{
		AbilityTarget |= target;
	}

	public void AdjustTargets(int amount)
	{
		AbilityTargets += amount;
	}

	public void AbilityAdjustRange(int amount)
	{
		AbilityRange += amount;

		SingleTargetRange += amount;
	}

	public void AbilitySetRangeType(RangeType rangeType)
	{
		AbilityRangeType = rangeType;

		SingleTargetRangeType = rangeType;
	}

	public void AbilityAddCondition(ConditionModel conditionModel)
	{
		if(conditionModel.CanStack)
		{
			AbilityConditionModels.Add(conditionModel);
			SingleTargetConditionModels.Add(conditionModel);
		}
		else
		{
			AbilityConditionModels.AddIfNew(conditionModel);
			SingleTargetConditionModels.AddIfNew(conditionModel);
		}
	}

	public void AbilityRemoveCondition(ConditionModel conditionModel)
	{
		AbilityConditionModels.Remove(conditionModel);

		SingleTargetConditionModels.Remove(conditionModel);
	}

	public void AbilityAddAOEPattern(AOEPattern aoePattern)
    {
		AbilityAOEPattern = aoePattern;
    }

	public void AbilityAdjustPush(int amount)
	{
		AbilityPush += amount;

		SingleTargetPush += amount;
	}

	public void AbilityAdjustPull(int amount)
	{
		AbilityPull += amount;

		SingleTargetPull += amount;
	}

	public void AbilityAdjustSwing(int amount)
	{
		AbilitySwing += amount;

		SingleTargetSwing += amount;
	}

	public void SingleTargetAdjustRange(int amount)
	{
		SingleTargetRange += amount;
	}

	public void SingleTargetSetRangeType(RangeType rangeType)
	{
		SingleTargetRangeType = rangeType;
	}

	public void SingleTargetAddCondition(ConditionModel conditionModel)
	{
		if(conditionModel.CanStack)
		{
			SingleTargetConditionModels.Add(conditionModel);
		}
		else
		{
			SingleTargetConditionModels.AddIfNew(conditionModel);
		}
	}

	public void SingleTargetRemoveCondition(ConditionModel conditionModel)
	{
		SingleTargetConditionModels.Remove(conditionModel);
	}

	public void SingleTargetAdjustPush(int amount)
	{
		SingleTargetPush += amount;
	}

	public void SingleTargetAdjustPull(int amount)
	{
		SingleTargetPull += amount;
	}

	public void SingleTargetAdjustSwing(int amount)
	{
		SingleTargetSwing += amount;
	}
}

/// <summary>
/// An <see cref="Ability{T}"/> that is considered a targeted ability as per the rules; that targets figures with given restrictions.
/// </summary>
public abstract class TargetedAbility<T, TSingleTargetState> : Ability<T>
	where T : TargetedAbilityState<TSingleTargetState>, new()
	where TSingleTargetState : SingleTargetState, new()
{
	private static readonly List<Hex> HexCache = new List<Hex>();

	private Func<T, string> _getTargetingHintText;

	public int Range { get; private set; } = 1;
	public RangeType RangeType { get; private set; } = RangeType.Melee;
	public bool RequiresLineOfSight { get; private set; } = true;
	public Target Target { get; protected set; } = Target.Enemies;
	public int Targets { get; private set; } = 1;
	public Hex TargetHex { get; private set; }
	public AOEPattern AOEPattern { get; private set; }
	public bool Mandatory { get; private set; }
	public int Push { get; private set; }
	public int Pull { get; private set; }
	public int Swing { get; private set; }

	public ConditionModel[] Conditions { get; private set; } = [];

	public Action<T, List<Figure>> CustomGetTargets { get; private set; }

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in TargetedAbility. Enables inheritors of TargetedAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending TargetedAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : Ability<T>.AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : TargetedAbility<T, TSingleTargetState>, new()
	{
		protected Target? _target;
		protected RangeType? _rangeType;
		protected Func<T, string> GetTargetingHintText;

		public TBuilder WithGetTargetingHintText(Func<T, string> getTargetingHintText)
		{
			GetTargetingHintText = getTargetingHintText;
			Obj._getTargetingHintText = getTargetingHintText;
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}

		public TBuilder WithRangeType(RangeType rangeType)
		{
			_rangeType = rangeType;
			Obj.RangeType = rangeType;
			return (TBuilder)this;
		}

		public TBuilder WithRequiresLineOfSight(bool requiresLineOfSight)
		{
			Obj.RequiresLineOfSight = requiresLineOfSight;
			return (TBuilder)this;
		}

		public TBuilder WithTarget(Target target)
		{
			_target = target;
			Obj.Target = target;
			return (TBuilder)this;
		}

		public TBuilder WithTargets(int targets)
		{
			Obj.Targets = targets;
			return (TBuilder)this;
		}

		public TBuilder WithTargetHex(Hex targetHex)
		{
			Obj.TargetHex = targetHex;
			return (TBuilder)this;
		}

		public TBuilder WithAOEPattern(AOEPattern aoePattern)
		{
			Obj.AOEPattern = aoePattern;
			return (TBuilder)this;
		}

		public TBuilder WithMandatory(bool mandatory)
		{
			Obj.Mandatory = mandatory;
			return (TBuilder)this;
		}

		public TBuilder WithPush(int push)
		{
			Obj.Push = push;
			return (TBuilder)this;
		}

		public TBuilder WithPull(int pull)
		{
			Obj.Pull = pull;
			return (TBuilder)this;
		}

		public TBuilder WithSwing(int swing)
		{
			Obj.Swing = swing;
			return (TBuilder)this;
		}

		public TBuilder WithConditions(params ConditionModel[] conditions)
		{
			Obj.Conditions = conditions;
			return (TBuilder)this;
		}

		public TBuilder WithCustomGetTargets(Action<T, List<Figure>> getTargets)
		{
			Obj.CustomGetTargets = getTargets;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.Target = _target ?? Target.Enemies;
			Obj.RangeType = _rangeType ?? (Obj.Range == 1 ? RangeType.Melee : RangeType.Range);
			Obj._getTargetingHintText = GetTargetingHintText ?? Obj.DefaultTargetingHintText;
			return base.Build();
		}
	}

	public TargetedAbility() { }

	protected override void InitializeState(T abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.AbilityTarget = Target;
		abilityState.AbilityTargets = Targets;
		abilityState.AbilityAOEPattern = AOEPattern;

		if(abilityState.AbilityTarget.HasFlag(Target.TargetAll))
		{
			abilityState.AbilityTargets = int.MaxValue;
		}

		abilityState.AbilityRange = Range;
		abilityState.AbilityRangeType = RangeType;
		abilityState.AbilityConditionModels = Conditions.ToList();
		abilityState.AbilityPush = Push;
		abilityState.AbilityPull = Pull;
		abilityState.AbilitySwing = Swing;
	}

	protected override async GDTask Perform(T abilityState)
	{
		Figure performer = abilityState.Performer;

		//await InitAbilityState(abilityState);

		if(abilityState.AbilityAOEPattern != null)
		{
			Dictionary<Vector2I, AOEHexType> aoeHexes = new Dictionary<Vector2I, AOEHexType>();

			//TODO: Add `during ability` scenario events to the aoe prompts so the range can be increased 
			if(abilityState.Authority is Character)
			{
				AOEPrompt.Answer aoeAnswer =
					await PromptManager.Prompt(new AOEPrompt(abilityState, abilityState.AbilityAOEPattern, TargetHex, null, () => "Select where to target"),
						abilityState.Authority);

				if(aoeAnswer.Skipped)
				{
					return;
				}

				for(int i = 0; i < aoeAnswer.HexCoords.Count; i++)
				{
					aoeHexes.Add(aoeAnswer.HexCoords[i], aoeAnswer.HexTypes[i]);
				}
			}
			else
			{
				Figure focus = await abilityState.ActionState.GetFocus();
				MonsterAOEPrompt.Answer aoeAnswer =
					await PromptManager.Prompt(
						new MonsterAOEPrompt(abilityState, abilityState.AbilityAOEPattern, abilityState.AbilityRange, abilityState.AbilityRangeType, focus, null,
							() => "Select where to target"), abilityState.Authority);

				if(aoeAnswer.Skipped)
				{
					return;
				}

				for(int i = 0; i < aoeAnswer.HexCoords.Count; i++)
				{
					aoeHexes.Add(aoeAnswer.HexCoords[i], aoeAnswer.HexTypes[i]);
				}
			}

			abilityState.AOEHexes = aoeHexes;
		}

		Action<List<Figure>> getValidTargets = figures =>
		{
			if(abilityState.AbilityTarget == Target.Self)
			{
				figures.Add(performer);
			}
			else if(CustomGetTargets != null)
			{
				CustomGetTargets(abilityState, figures);
			}
			else if(abilityState.AOEHexes != null)
			{
				foreach(Hex redAOEHex in abilityState.GetRedAOEHexes())
				{
					figures.AddRange(redAOEHex.GetHexObjectsOfType<Figure>());
				}
			}
			else if(TargetHex != null)
			{
				figures.AddRange(TargetHex.GetHexObjectsOfType<Figure>());
			}
			else
			{
				HexCache.Clear();
				RangeHelper.FindHexesInRange(performer.Hex, abilityState.SingleTargetRange, true, HexCache);

				foreach(Hex hex in HexCache)
				{
					figures.AddRange(hex.GetHexObjectsOfType<Figure>());
				}
			}

			for(int i = figures.Count - 1; i >= 0; i--)
			{
				Figure figure = figures[i];

				bool remove = false;

				// Remove any duplicates
				for(int j = 0; j < i - 1; j++)
				{
					if(figures[j] == figure)
					{
						remove = true;
					}
				}

				if(abilityState.Authority.AlliedWith(figure, false) && 
					!abilityState.AbilityTarget.HasFlag(Target.Self) && 
					!abilityState.AbilityTarget.HasFlag(Target.Allies))
				{
					remove = true;
				}

				if(abilityState.AbilityTarget.HasFlag(Target.Enemies) && abilityState.Authority == figure)
				{
					remove = true;
				}

				if(!abilityState.AbilityTarget.HasFlag(Target.Enemies) && abilityState.Authority.EnemiesWith(figure))
				{
					remove = true;
				}

				if(!abilityState.AbilityTarget.HasFlag(Target.Self) && abilityState.Performer == figure)
				{
					remove = true;
				}

				if(abilityState.AbilityTarget.HasFlag(Target.SelfCountsForTargets) && 
					abilityState.SingleTargetStates.Count + 1 == abilityState.AbilityTargets &&
				   	!abilityState.UniqueTargetedFigures.Contains(performer) && abilityState.Performer != figure)
				{
					remove = true;
				}

				if(!abilityState.AbilityTarget.HasFlag(Target.MustTargetSameWithAllTargets) && abilityState.UniqueTargetedFigures.Contains(figure))
				{
					remove = true;
				}

				if(abilityState.AbilityTarget.HasFlag(Target.MustTargetSameWithAllTargets) && abilityState.UniqueTargetedFigures.Count > 0 &&
				   abilityState.UniqueTargetedFigures[0] != figure)
				{
					remove = true;
				}

				if(abilityState.AbilityTarget.HasFlag(Target.MustTargetCharacters) && figure is not Character)
				{
					remove = true;
				}

				if(RequiresLineOfSight && !GameController.Instance.Map.HasLineOfSight(abilityState.Performer.Hex, figure.Hex))
				{
					remove = true;
				}

				ScenarioCheckEvents.CanBeTargetedCheck.Parameters canBeTargetedParameters =
					ScenarioCheckEvents.CanBeTargetedCheckEvent.Fire(
						new ScenarioCheckEvents.CanBeTargetedCheck.Parameters(abilityState, performer, figure));

				if(!canBeTargetedParameters.CanBeTargeted)
				{
					remove = true;
				}

				if(figure.IsDead)
				{
					remove = true;
				}

				if(remove)
				{
					figures.RemoveAt(i);
				}
			}
		};

		while(true)
		{
			if(abilityState.Blocked || performer.IsDead)
			{
				break;
			}

			InitAbilityStateForSingleTarget(abilityState);

			EffectCollection duringTargetedAbilityEffectCollection = CreateDuringTargetedAbilityEffectCollection(abilityState);

			int figureReferenceId;

			if(abilityState.Authority is Character)
			{
				bool autoSelectIfOne = Mandatory || 
					abilityState.AbilityTarget == Target.Self || 
					(TargetHex != null && abilityState.AbilityAOEPattern == null);
				TargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
					new TargetSelectionPrompt(getValidTargets, autoSelectIfOne, Mandatory, duringTargetedAbilityEffectCollection,
						() => _getTargetingHintText(abilityState)), abilityState.Authority);

				if(targetAnswer.Skipped)
				{
					break;
				}

				figureReferenceId = targetAnswer.FigureReferenceId;
			}
			else
			{
				//List<FocusNode> bestFocusNodes = await abilityState.Performer.GetBestFocusNodes();
				//Figure focus = bestFocusNodes.Count > 0 ? bestFocusNodes[0].Focus : null;
				Figure focus = await abilityState.ActionState.GetFocus();
				MonsterTargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
					new MonsterTargetSelectionPrompt(getValidTargets, true, focus, duringTargetedAbilityEffectCollection,
						() => _getTargetingHintText(abilityState)), abilityState.Authority);

				if(targetAnswer.Skipped)
				{
					break;
				}

				figureReferenceId = targetAnswer.FigureReferenceId;
			}

			Figure target = GameController.Instance.ReferenceManager.Get<Figure>(figureReferenceId);
			abilityState.AddSingleTargetState(target);
			//abilityState.Target = target;
			abilityState.UniqueTargetedFigures.AddIfNew(target);
			abilityState.TargetedHexes.AddIfNew(target.Hex);

			abilityState.SetPerformed();

			// if(duringTargetedAbilityParameters != null)
			// {
			// 	SyncDuringTargetedAbilityParameters(abilityState, duringTargetedAbilityParameters);
			// }

			await AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);

			await ApplyConditions(abilityState, target, abilityState.SingleTargetConditionModels);

			await AfterConditionsApplied(abilityState, target);

			// Pull
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetPull > 0)
			{
				await ForcedMovement(abilityState, performer.Hex, target, abilityState.SingleTargetPull, ForcedMovementType.Pull,
					() => $"Select a path to {Icons.HintText(Icons.Pull)}{abilityState.SingleTargetPull} target");
			}

			// Push
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetPush > 0)
			{
				await ForcedMovement(abilityState, performer.Hex, target, abilityState.SingleTargetPush, ForcedMovementType.Push,
					() => $"Select a path to {Icons.HintText(Icons.Push)}{abilityState.SingleTargetPush} target");
			}

			// Swing
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetSwing > 0)
			{
				await ForcedMovement(abilityState, performer.Hex, target, abilityState.SingleTargetSwing, ForcedMovementType.Swing,
					() => $"Select a path to {Icons.HintText(Icons.Swing)}{abilityState.SingleTargetSwing} target");
			}

			await AfterEffects(abilityState, target);

			if(performer.IsDestroyed)
			{
				break;
			}

			if(abilityState.AbilityAOEPattern != null)
			{
				if(abilityState.TargetedHexes.Count == abilityState.AbilityAOEPattern.Hexes.Count)
				{
					break;
				}
			}
			else if(abilityState.SingleTargetStates.Count == abilityState.AbilityTargets)
			{
				break;
			}
		}
	}

	// protected virtual async GDTask InitAbilityState(T abilityState)
	// {
	// 	abilityState.AbilityTargets = Targets;
	// 	if(TargetAll)
	// 	{
	// 		abilityState.AbilityTargets = int.MaxValue;
	// 	}
	//
	// 	abilityState.AbilityRange = Range;
	// 	abilityState.AbilityRangeType = RangeType;
	// 	abilityState.AbilityConditionModels = Conditions.ToList();
	// 	abilityState.AbilityPush = Push;
	// 	abilityState.AbilityPull = Pull;
	//
	// 	await GDTask.CompletedTask;
	// }

	protected virtual void InitAbilityStateForSingleTarget(T abilityState)
	{
		abilityState.SingleTargetRange = abilityState.AbilityRange;
		abilityState.SingleTargetRangeType = abilityState.AbilityRangeType;
		abilityState.SingleTargetConditionModels = abilityState.AbilityConditionModels.ToList();
		abilityState.SingleTargetPush = abilityState.AbilityPush;
		abilityState.SingleTargetPull = abilityState.AbilityPull;
		abilityState.SingleTargetSwing = abilityState.AbilitySwing;
	}

	protected virtual EffectCollection CreateDuringTargetedAbilityEffectCollection(T abilityState)
	{
		return null;
	}

	protected virtual async GDTask AfterTargetConfirmedBeforeConditionsApplied(T abilityState, Figure target)
	{
		await GDTask.CompletedTask;
	}

	protected virtual async GDTask AfterConditionsApplied(T abilityState, Figure target)
	{
		await GDTask.CompletedTask;
	}

	protected virtual async GDTask AfterEffects(T abilityState, Figure target)
	{
		await GDTask.CompletedTask;
	}

	protected async GDTask ForcedMovement(T abilityState, Hex origin, Figure target, int distance, ForcedMovementType type, Func<string> hintText)
	{
		ScenarioCheckEvents.ImmuneToForcedMovementCheck.Parameters immuneToForcedMovementParameters =
			ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Fire(
				new ScenarioCheckEvents.ImmuneToForcedMovementCheck.Parameters(target));

		if(immuneToForcedMovementParameters.ImmuneToForcedMovement)
		{
			return;
		}

		List<Vector2I> path = null;
		SwingDirectionType? requiredDirection = null;

		if(type == ForcedMovementType.Swing)
		{
			ScenarioEvents.SwingDirectionCheck.Parameters parameters =
				await ScenarioEvents.SwingDirectionCheckEvent.CreatePrompt(
					new ScenarioEvents.SwingDirectionCheck.Parameters(abilityState));
			requiredDirection = parameters.RequiredDirection;
		}

		if(abilityState.Authority is Character)
		{
			ForcedMovementPrompt.Answer forcedMovementAnswer = await PromptManager.Prompt(
				new ForcedMovementPrompt(abilityState, origin, target, distance, type, null, hintText, requiredDirection), abilityState.Authority);

			if(!forcedMovementAnswer.Skipped)
			{
				path = forcedMovementAnswer.Path;
			}
		}
		else
		{
			MonsterForcedMovementPrompt.Answer answer = await PromptManager.Prompt(
				new MonsterForcedMovementPrompt(abilityState, origin, target, distance, type, null, hintText, requiredDirection),
				abilityState.Authority);

			if(!answer.Skipped)
			{
				path = answer.Path;
			}
		}

		if(path != null)
		{
			target.ZIndex = 100;

			for(int i = 0; i < path.Count; i++)
			{
				Vector2I coords = path[i];
				Hex hex = GameController.Instance.Map.GetHex(coords);
				abilityState.SingleTargetState.ForcedMovementHexes.Add(hex);

				await target.TweenGlobalPosition(hex.GlobalPosition, 0.2f).PlayFastForwardableAsync();
				await AbilityCmd.EnterHex(abilityState, target, abilityState.Authority, hex, true);

				ScenarioEvents.MoveTogetherCheck.Parameters moveTogetherCheckParameters =
					await ScenarioEvents.MoveTogetherCheckEvent.CreatePrompt(new ScenarioEvents.MoveTogetherCheck.Parameters(target));

				if(moveTogetherCheckParameters.OtherFigure != null)
				{
					await target.TweenGlobalPosition(hex.GlobalPosition, 0.2f).PlayFastForwardableAsync();
					await AbilityCmd.EnterHex(abilityState, moveTogetherCheckParameters.OtherFigure, abilityState.Authority, hex, true);
				}
			}

			target.ZIndex = target.DefaultZIndex;
		}
	}

	private async GDTask ApplyConditions(T abilityState, Figure target, List<ConditionModel> conditionModels)
	{
		if(!target.IsDestroyed)
		{
			await AbilityCmd.AddConditions(abilityState, target, conditionModels);
		}
	}

	protected virtual string DefaultTargetingHintText(T abilityState)
	{
		return "Select a target";
	}
}