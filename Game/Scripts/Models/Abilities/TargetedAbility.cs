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
	public List<Hex> PullHexes { get; } = new List<Hex>();
	public List<Hex> PushHexes { get; } = new List<Hex>();
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

public abstract class TargetedAbilityState : AbilityState, IConditionsAbilityState
{
	public List<Figure> UniqueTargetedFigures { get; } = new List<Figure>();
	public List<Hex> TargetedHexes { get; } = new List<Hex>();
	public List<AOEHex> TargetedAOEHexes { get; set; }

	public Target AbilityTarget { get; set; }
	public int AbilityTargets { get; set; }
	public Action<TargetedAbilityState, List<Figure>> AbilityCustomGetTargets { get; set; }
	public Func<TargetedAbilityState, Figure, bool> AbilityFilterTargets { get; set; }
	public AOEPattern AbilityAOEPattern { get; set; }
	public Hex AbilityPerformHex { get; set; }

	public RangeType AbilityRangeType { get; set; }
	public int AbilityRange { get; set; }
	public int AbilityMinRange { get; set; }
	public List<ConditionModel> AbilityConditionModels { get; set; }
	public int AbilityPush { get; set; }
	public int AbilityPull { get; set; }
	public int AbilitySwing { get; set; }

	public RangeType SingleTargetRangeType { get; set; }
	public int SingleTargetRange { get; set; }
	public int SingleTargetMinRange { get; set; }
	public List<ConditionModel> SingleTargetConditionModels { get; set; }
	public int SingleTargetPush { get; set; }
	public int SingleTargetPull { get; set; }
	public int SingleTargetSwing { get; set; }

	public abstract Figure Target { get; }

	public Hex GetPerformHex => AbilityPerformHex ?? Performer.Hex;

	public IEnumerable<Hex> GetEmptyAOEHexes()
	{
		if(TargetedAOEHexes == null)
		{
			yield break;
		}

		foreach(AOEHex aoeHex in TargetedAOEHexes)
		{
			Hex hex = GameController.Instance.Map.GetHex(aoeHex.Coords);

			if(hex != null && aoeHex.Type.HasFlag(AOEHexType.Empty))
			{
				yield return hex;
			}
		}
	}

	public IEnumerable<Hex> GetRedAOEHexes()
	{
		if(TargetedAOEHexes == null)
		{
			yield break;
		}

		foreach(AOEHex aoeHex in TargetedAOEHexes)
		{
			Hex hex = GameController.Instance.Map.GetHex(aoeHex.Coords);

			if(hex != null && aoeHex.Type.HasFlag(AOEHexType.Red))
			{
				yield return hex;
			}
		}
	}

	public IEnumerable<Hex> GetYellowAOEHexes()
	{
		if(TargetedAOEHexes == null)
		{
			yield break;
		}

		foreach(AOEHex aoeHex in TargetedAOEHexes)
		{
			Hex hex = GameController.Instance.Map.GetHex(aoeHex.Coords);

			if(hex != null && aoeHex.Type.HasFlag(AOEHexType.Yellow))
			{
				yield return hex;
			}
		}
	}

	public IEnumerable<Hex> GetCustomMarkedHexes(string customMark)
	{
		if(TargetedAOEHexes == null)
		{
			yield break;
		}

		foreach(AOEHex aoeHex in TargetedAOEHexes)
		{
			Hex hex = GameController.Instance.Map.GetHex(aoeHex.Coords);

			if(hex != null && aoeHex.CustomMark == customMark)
			{
				yield return hex;
			}
		}
	}

	public void SetAbilityCustomTargets(Action<TargetedAbilityState, List<Figure>> customGetTargets)
	{
		AbilityCustomGetTargets = customGetTargets;
	}

	public void SetAbilityFilterTargets(Func<TargetedAbilityState, Figure, bool> filterTargets)
	{
		AbilityFilterTargets = filterTargets;
	}

	public async GDTask SetPerformHex(Action<List<Hex>> getValidHexes, bool mandatory = true)
	{
		Hex hex = await AbilityCmd.SelectHex(this, getValidHexes, mandatory, "Select a hex to perform this ability from");
		if(hex != null)
		{
			SetPerformHex(hex);
		}
	}

	public void SetPerformHex(Hex hex)
	{
		AbilityPerformHex = hex ?? Performer.Hex;
	}

	public void SetTarget(Target target)
	{
		AbilityTarget = target;
		if(target.HasFlag(global::Target.TargetAll))
		{
			AbilityTargets = int.MaxValue;
		}
	}

	public void AdjustTarget(Target target)
	{
		AbilityTarget |= target;
		if(target.HasFlag(global::Target.TargetAll))
		{
			AbilityTargets = int.MaxValue;
		}
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

	public void AbilityAdjustMinRange(int amount)
	{
		AbilityMinRange += amount;

		SingleTargetMinRange += amount;
	}

	public void AbilitySetRangeType(RangeType rangeType)
	{
		AbilityRangeType = rangeType;

		SingleTargetRangeType = rangeType;
	}

	public void AbilityAddCondition(ConditionModel conditionModel)
	{
		if(conditionModel.CanBeAppliedMultipleTimesOnSingleTarget)
		{
			AbilityConditionModels.Add(conditionModel);
			SingleTargetConditionModels?.Add(conditionModel);
		}
		else
		{
			AbilityConditionModels.AddIfNew(conditionModel);
			SingleTargetConditionModels?.AddIfNew(conditionModel);
		}
	}

	public void AbilityRemoveCondition(ConditionModel conditionModel)
	{
		AbilityConditionModels.Remove(conditionModel);

		SingleTargetConditionModels.Remove(conditionModel);
	}

	public void AbilitySetAOEPattern(AOEPattern aoePattern)
	{
		AbilityAOEPattern = aoePattern;
	}

	public void AbilityAddAOEHex(AOEHex aoeHex)
	{
		AbilityAOEPattern.LocalHexes.Add(aoeHex);
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
		if(conditionModel.CanBeAppliedMultipleTimesOnSingleTarget)
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
/// An <see cref="Ability{State}"/> that is considered a targeted ability as per the rules; that targets figures with given restrictions.
/// </summary>
public abstract class TargetedAbility<T, TSingleTargetState> : Ability<T>, ITargetedAbility
	where T : TargetedAbilityState<TSingleTargetState>, new()
	where TSingleTargetState : SingleTargetState, new()
{
	private static readonly List<Hex> HexCache = new List<Hex>();
	private static readonly List<Node2D> PreviousParents = new List<Node2D>();

	private Func<T, string> _getTargetingHintText;

	public DynamicInt Range { get; private set; } = 1;
	public int MinRange { get; private set; } = 0;
	public DynamicRangeType TypeOfRange { get; private set; } = RangeType.Melee;
	public bool RequiresLineOfSight { get; private set; } = true;
	public DynamicTarget TargetType { get; protected set; } = Target.Enemies;
	public DynamicInt Targets { get; private set; } = 1;
	public Hex TargetHex { get; private set; }
	public DynamicAOEPattern AOEPattern { get; private set; }
	public bool Mandatory { get; private set; }
	public int Push { get; private set; }
	public int Pull { get; private set; }
	public int Swing { get; private set; }

	public ConditionModel[] Conditions { get; private set; } = [];

	public Action<T, List<Figure>> CustomGetTargets { get; private set; }
	public Func<T, Hex> CustomGetPerformHex { get; private set; }
	public bool CanTargetNonFigures { get; private set; }
	public Func<T, Figure, bool> FilterTargets { get; private set; }

	public bool IsMultiTarget =>
		Targets.GetValue() > 1 ||
		TargetType.GetValue().HasFlag(Target.TargetAll) ||
		(AOEPattern != null && AOEPattern.GetValue().LocalHexes.Count(hex => hex.Type == AOEHexType.Red) > 1);

	public AOEPattern AbilityAOEPattern =>
		AOEPattern?.GetValue();

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
		protected DynamicTarget _target;
		protected DynamicRangeType _rangeType;
		protected Func<T, string> GetTargetingHintText;

		public TBuilder WithGetTargetingHintText(Func<T, string> getTargetingHintText)
		{
			GetTargetingHintText = getTargetingHintText;
			Obj._getTargetingHintText = getTargetingHintText;
			return (TBuilder)this;
		}

		public TBuilder WithRange(DynamicInt range, params RangeSquare[] enhancementMarks)
		{
			Obj.Range = range;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithMinRange(int minRange)
		{
			Obj.MinRange = minRange;
			return (TBuilder)this;
		}

		public TBuilder WithInfiniteRange()
		{
			Obj.Range = RangeHelper.InfiniteRange;
			return (TBuilder)this;
		}

		public TBuilder WithRangeType(DynamicRangeType rangeType)
		{
			_rangeType = rangeType;
			Obj.TypeOfRange = rangeType;
			return (TBuilder)this;
		}

		public TBuilder WithRequiresLineOfSight(bool requiresLineOfSight)
		{
			Obj.RequiresLineOfSight = requiresLineOfSight;
			return (TBuilder)this;
		}

		public TBuilder WithTarget(DynamicTarget target)
		{
			_target = target;
			Obj.TargetType = target;
			return (TBuilder)this;
		}

		public TBuilder WithTargets(DynamicInt targets, params TargetsSquare[] enhancementMarks)
		{
			Obj.Targets = targets;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithTargetHex(Hex targetHex)
		{
			Obj.TargetHex = targetHex;
			return (TBuilder)this;
		}

		public TBuilder WithAOEPattern(DynamicAOEPattern aoePattern, params AOEHexMark[] enhancementMarks)
		{
			Obj.AOEPattern = aoePattern;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithMandatory(bool mandatory)
		{
			Obj.Mandatory = mandatory;
			return (TBuilder)this;
		}

		public TBuilder WithPush(int push, params PushEnhancementMark[] enhancementMarks)
		{
			Obj.Push = push;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithPull(int pull, params PullEnhancementMark[] enhancementMarks)
		{
			Obj.Pull = pull;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithSwing(int swing, params SwingEnhancementMark[] enhancementMarks)
		{
			Obj.Swing = swing;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithConditions(ConditionModel condition, params ConditionEnhancementMark[] enhancementMarks)
		{
			return WithConditions([condition], enhancementMarks);
		}

		public TBuilder WithConditions(ConditionModel[] conditions, params ConditionEnhancementMark[] enhancementMarks)
		{
			Obj.Conditions = conditions;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithCustomGetTargets(Action<T, List<Figure>> getTargets)
		{
			Obj.CustomGetTargets = getTargets;
			return (TBuilder)this;
		}

		public TBuilder WithCustomGetPerformHex(Func<T, Hex> getPerformHex)
		{
			Obj.CustomGetPerformHex = getPerformHex;
			return (TBuilder)this;
		}

		public TBuilder WithCanTargetNonFigures()
		{
			Obj.CanTargetNonFigures = true;
			return (TBuilder)this;
		}

		public TBuilder WithFilterTargets(Func<T, Figure, bool> filterTargets)
		{
			Obj.FilterTargets = filterTargets;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.TargetType = _target ?? Target.Enemies;
			Obj.TypeOfRange = _rangeType ?? new(() => Obj.Range.GetValue() == 1 ? RangeType.Melee : RangeType.Range);
			Obj._getTargetingHintText = GetTargetingHintText ?? Obj.DefaultTargetingHintText;
			return base.Build();
		}
	}

	public TargetedAbility() { }

	protected override void InitializeState(T abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.AbilityTarget = TargetType.GetValue();
		abilityState.AbilityTargets = Targets.GetValue();
		abilityState.AbilityAOEPattern = AOEPattern?.GetValue();

		if(abilityState.AbilityTarget.HasFlag(Target.TargetAll))
		{
			abilityState.AbilityTargets = int.MaxValue;
		}

		abilityState.AbilityPerformHex = null;

		abilityState.AbilityRange = Range.GetValue();
		abilityState.AbilityMinRange = MinRange;
		abilityState.AbilityRangeType = TypeOfRange.GetValue();
		abilityState.AbilityConditionModels = Conditions.ToList();
		abilityState.AbilityPush = Push;
		abilityState.AbilityPull = Pull;
		abilityState.AbilitySwing = Swing;
		abilityState.AbilityCustomGetTargets = CustomGetTargets != null
			? (state, figures) => CustomGetTargets((T)state, figures)
			: null;
		abilityState.AbilityFilterTargets = FilterTargets != null
			? (state, figures) => FilterTargets((T)state, figures)
			: null;
	}

	protected override async GDTask Perform(T abilityState)
	{
		if(CustomGetPerformHex != null)
		{
			Hex performHex = CustomGetPerformHex(abilityState);
			abilityState.SetPerformHex(performHex);
		}

		Figure performer = abilityState.Performer;

		if(abilityState.AbilityAOEPattern != null)
		{
			List<AOEHex> aoeHexes;

			//TODO: Add `during ability` scenario events to the aoe prompts so the range can be increased 
			if(abilityState.Authority is Character)
			{
				AOEPrompt.Answer aoeAnswer =
					await PromptManager.Prompt(
						new AOEPrompt(abilityState.Performer, abilityState.AbilityAOEPattern, TargetHex, null, () => "Select where to target",
							abilityState.AbilityRange),
						abilityState.Authority);

				if(aoeAnswer.Skipped)
				{
					return;
				}

				aoeHexes = aoeAnswer.AOEHexes;
			}
			else
			{
				Figure focus = await abilityState.ActionState.GetFocus(abilityState);

				MonsterAOEPrompt.Answer aoeAnswer =
					await PromptManager.Prompt(
						new MonsterAOEPrompt(abilityState, abilityState.AbilityAOEPattern, abilityState.AbilityRange, abilityState.AbilityRangeType,
							focus, null,
							() => "Select where to target"), abilityState.Authority);

				if(aoeAnswer.Skipped)
				{
					return;
				}

				aoeHexes = aoeAnswer.AOEHexes;
			}

			abilityState.TargetedAOEHexes = aoeHexes;
		}

		int targetsOutOfAOE = 0;
		//TODO: Check this out
		Action<List<Figure>> getValidTargets = figures => GetValidTargets(abilityState, figures, targetsOutOfAOE);

		while(true)
		{
			if(abilityState.Blocked || performer.IsDead)
			{
				break;
			}

			InitAbilityStateForSingleTarget(abilityState);

			EffectCollection duringTargetedAbilityEffectCollection = CreateDuringTargetedAbilityEffectCollection(abilityState);

			Figure target = null;

			if(abilityState.Authority is Character)
			{
				bool autoSelectIfOne =
					Mandatory ||
					abilityState.AbilityTarget == Target.Self ||
					(TargetHex != null && abilityState.AbilityAOEPattern == null);
				target = await AbilityCmd.SelectFigure(abilityState, getValidTargets, mandatory: Mandatory,
					autoSelectIfOne: autoSelectIfOne, autoSkipIfNone: false,
					duringTargetedAbilityEffectCollection,
					() => _getTargetingHintText(abilityState));
			}
			else
			{
				Figure focus = await abilityState.ActionState.GetFocus(abilityState);

				MonsterTargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
					new MonsterTargetSelectionPrompt(getValidTargets, true, focus, duringTargetedAbilityEffectCollection,
						() => _getTargetingHintText(abilityState)), abilityState.Authority);

				target = targetAnswer.Skipped ? null : GameController.Instance.ReferenceManager.Get<Figure>(targetAnswer.FigureReferenceId);
			}

			if(target == null)
			{
				break;
			}

			abilityState.AddSingleTargetState(target);
			abilityState.UniqueTargetedFigures.AddIfNew(target);
			abilityState.TargetedHexes.AddIfNew(target.Hex);
			if(!abilityState.GetRedAOEHexes().Contains(target.Hex))
			{
				targetsOutOfAOE++;
			}

			abilityState.SetPerformed();

			await AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);

			await ApplyConditions(abilityState, target, abilityState.SingleTargetConditionModels);

			await AfterConditionsApplied(abilityState, target);

			// Pull
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetPull > 0)
			{
				await ForcedMovement(abilityState, abilityState.GetPerformHex, target, abilityState.SingleTargetPull, ForcedMovementType.Pull,
					() => $"Select a path to {Icons.HintText(Icons.Pull)}{abilityState.SingleTargetPull} target");
			}

			// Push
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetPush > 0)
			{
				await ForcedMovement(abilityState, abilityState.GetPerformHex, target, abilityState.SingleTargetPush, ForcedMovementType.Push,
					() => $"Select a path to {Icons.HintText(Icons.Push)}{abilityState.SingleTargetPush} target");
			}

			// Swing
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetSwing > 0)
			{
				await ForcedMovement(abilityState, abilityState.GetPerformHex, target, abilityState.SingleTargetSwing, ForcedMovementType.Swing,
					() => $"Select a path to {Icons.HintText(Icons.Swing)}{abilityState.SingleTargetSwing} target");
			}

			await AfterEffects(abilityState, target);

			if(performer.IsDestroyed)
			{
				break;
			}

			if(abilityState.AbilityAOEPattern != null)
			{
				if(abilityState.TargetedHexes.Count == abilityState.AbilityAOEPattern.LocalHexes.Count &&
				   targetsOutOfAOE == abilityState.AbilityTargets - 1)
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

	protected virtual void InitAbilityStateForSingleTarget(T abilityState)
	{
		abilityState.SingleTargetRange = abilityState.AbilityRange;
		abilityState.SingleTargetMinRange = abilityState.AbilityMinRange;
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
				if(type == ForcedMovementType.Pull)
				{
					abilityState.SingleTargetState.PullHexes.Add(hex);
				}

				if(type == ForcedMovementType.Push)
				{
					abilityState.SingleTargetState.PushHexes.Add(hex);
				}

				ScenarioEvents.MoveTogether.Parameters moveTogetherCheckParameters =
					await ScenarioEvents.MoveTogetherEvent.CreatePrompt(new ScenarioEvents.MoveTogether.Parameters(abilityState, target, hex));

				await AbilityCmd.ExitHex(abilityState, target, abilityState.Authority);

				PreviousParents.Clear();
				Node2D moveParent = GameController.Instance.MoveParent;
				moveParent.SetGlobalPosition(target.Hex.GlobalPosition);
				PreviousParents.Add(target.GetParent<Node2D>());
				target.Reparent(moveParent);
				foreach(Figure otherFigure in moveTogetherCheckParameters.OtherFigures)
				{
					PreviousParents.Add(otherFigure.GetParent<Node2D>());
					otherFigure.Reparent(moveParent);
				}

				await moveParent.TweenGlobalPosition(hex.GlobalPosition, 0.2f).PlayFastForwardableAsync();

				target.Reparent(PreviousParents[0]);
				PreviousParents.RemoveAt(0);
				foreach(Figure otherFigure in moveTogetherCheckParameters.OtherFigures)
				{
					otherFigure.Reparent(PreviousParents[0]);
					PreviousParents.RemoveAt(0);
				}

				await AbilityCmd.EnterHex(abilityState, target, abilityState.Authority, hex, true, true);

				foreach(Figure otherFigure in moveTogetherCheckParameters.OtherFigures)
				{
					await AbilityCmd.ExitHex(abilityState, otherFigure, abilityState.Authority);
					await AbilityCmd.EnterHex(abilityState, otherFigure, abilityState.Authority, hex,
						moveTogetherCheckParameters.TriggerHexEffects, false);
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

	protected virtual void GetValidTargets(T abilityState, List<Figure> figures, int targetsOutOfAOE)
	{
		Figure performer = abilityState.Performer;

		if(abilityState.AbilityTarget == Target.Self)
		{
			figures.Add(performer);
		}
		else if(abilityState.AbilityCustomGetTargets != null)
		{
			abilityState.AbilityCustomGetTargets(abilityState, figures);
		}
		else if(abilityState.TargetedAOEHexes != null)
		{
			foreach(Hex redAOEHex in abilityState.GetRedAOEHexes())
			{
				figures.AddRange(redAOEHex.GetHexObjectsOfType<Figure>());
			}

			if(targetsOutOfAOE < abilityState.AbilityTargets - 1)
			{
				HexCache.Clear();
				RangeHelper.FindHexesInRange(performer.Hex, abilityState.SingleTargetRange, true, HexCache,
					minRange: abilityState.SingleTargetMinRange);

				foreach(Hex hex in HexCache)
				{
					figures.AddRange(hex.GetHexObjectsOfType<Figure>());
				}
			}
		}
		else if(TargetHex != null)
		{
			figures.AddRange(TargetHex.GetHexObjectsOfType<Figure>());
		}
		else
		{
			HexCache.Clear();
			RangeHelper.FindHexesInRange(abilityState.GetPerformHex, abilityState.SingleTargetRange, true, HexCache,
				minRange: abilityState.SingleTargetMinRange);

			foreach(Hex hex in HexCache)
			{
				figures.AddRange(hex.GetHexObjectsOfType<Figure>());
			}
		}

		bool shouldFilterTargets = abilityState.AbilityFilterTargets != null;

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

			if(abilityState.Authority.AlliedWith(figure, true) &&
			   !abilityState.AbilityTarget.HasFlag(Target.Self) &&
			   !abilityState.AbilityTarget.HasFlag(Target.Allies))
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

			if(RequiresLineOfSight && !GameController.Instance.Map.HasLineOfSight(abilityState.GetPerformHex, figure.Hex))
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

			if(!CanTargetNonFigures && !figure.IsFigure)
			{
				remove = true;
			}

			if(shouldFilterTargets && !abilityState.AbilityFilterTargets(abilityState, figure))
			{
				remove = true;
			}

			if(remove)
			{
				figures.RemoveAt(i);
			}
		}
	}

	protected virtual string DefaultTargetingHintText(T abilityState)
	{
		return "Select a target";
	}
}