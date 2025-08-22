using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweensGodot.Extensions;

public class SingleTargetState
{
	public Figure Target;
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

	public int AbilityTargets { get; set; }

	public RangeType AbilityRangeType { get; set; }
	public int AbilityRange { get; set; }
	public List<ConditionModel> AbilityConditionModels { get; set; }
	public int AbilityPush { get; set; }
	public int AbilityPull { get; set; }

	public RangeType SingleTargetRangeType { get; set; }
	public int SingleTargetRange { get; set; }
	public List<ConditionModel> SingleTargetConditionModels { get; set; }
	public int SingleTargetPush { get; set; }
	public int SingleTargetPull { get; set; }

	public abstract Figure Target { get; }

	public IEnumerable<Hex> GetRedAOEHexes()
	{
		foreach((Vector2I coords, AOEHexType type) in AOEHexes)
		{
			Hex hex = GameController.Instance.Map.GetHex(coords);

			if(hex != null && type == AOEHexType.Red)
			{
				yield return hex;
			}
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
}

/// <summary>
/// An <see cref="Ability{T}"/> that is considered a targeted ability as per the rules; that targets a specific figure.
/// </summary>
public abstract class TargetedAbility<T, TSingleTargetState> : Ability<T>
	where T : TargetedAbilityState<TSingleTargetState>, new()
	where TSingleTargetState : SingleTargetState, new()
{
	private static readonly List<Hex> HexCache = new List<Hex>();

	private Func<T, string> GetTargetingHintText { get; set; }

	public int Range { get; protected set; } = 1;
	public RangeType RangeType { get; protected set; } = RangeType.Melee;
	public bool RequiresLineOfSight { get; protected set; } = true;
	public Target Target { get; protected set; } = Target.Enemies;
	public int Targets { get; protected set; } = 1;
	public Hex TargetHex { get; protected set; }
	public AOEPattern AOEPattern { get; protected set; }
	public bool Mandatory { get; protected set; }
	public int Push { get; protected set; }
	public int Pull { get; protected set; }

	public ConditionModel[] Conditions { get; protected set; } = [];

	public Action<T, List<Figure>> CustomGetTargets { get; protected set; }

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
		protected Func<T, string> GetTargetingHintText;

		public TBuilder WithGetTargetingHintText(Func<T, string> getTargetingHintText)
		{
			GetTargetingHintText = getTargetingHintText;
			Obj.GetTargetingHintText = getTargetingHintText;
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}

		public TBuilder WithRangeType(RangeType rangeType)
		{
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
			Obj.GetTargetingHintText = GetTargetingHintText ?? Obj.DefaultTargetingHintText;
			return base.Build();
		}
	}

	public TargetedAbility() { }

	public TargetedAbility(int targets = 1, int? range = null, RangeType? rangeType = null,
		Target target = Target.Enemies,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, int push = 0, int pull = 0, ConditionModel[] conditions = null,
		Action<T, List<Figure>> customGetTargets = null,
		Func<T, GDTask> onAbilityStarted = null, Func<T, GDTask> onAbilityEnded = null, Func<T, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<T, string> getTargetingHintText = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed, conditionalAbilityCheck, abilityStartedSubscriptions,
			abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		Targets = targets;
		Range = range ?? 1;
		RangeType = rangeType ?? (Range == 1 ? RangeType.Melee : RangeType.Range);
		Target = target;

		RequiresLineOfSight = requiresLineOfSight;
		Mandatory = mandatory;
		TargetHex = targetHex;
		AOEPattern = aoePattern;
		Push = push;
		Pull = pull;

		Conditions = conditions ?? [];
		CustomGetTargets = customGetTargets;
		GetTargetingHintText = getTargetingHintText ?? DefaultTargetingHintText;
	}

	protected override void InitializeState(T abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.AbilityTargets = Targets;
		if(Target.HasFlag(Target.TargetAll))
		{
			abilityState.AbilityTargets = int.MaxValue;
		}

		abilityState.AbilityRange = Range;
		abilityState.AbilityRangeType = RangeType;
		abilityState.AbilityConditionModels = Conditions.ToList();
		abilityState.AbilityPush = Push;
		abilityState.AbilityPull = Pull;
	}

	protected override async GDTask Perform(T abilityState)
	{
		Figure performer = abilityState.Performer;

		List<Figure> customTargets = null; // = CustomTargets?.ToList();

		// if(MustTargetSelf && customTargets == null)
		// {
		// 	customTargets = [performer];
		// }
		if(Target == Target.Self)
		{
			customTargets = [performer];
		}

		//await InitAbilityState(abilityState);

		if(AOEPattern != null)
		{
			Dictionary<Vector2I, AOEHexType> aoeHexes = new Dictionary<Vector2I, AOEHexType>();

			//TODO: Add `during ability` scenario events to the aoe prompts so the range can be increased 
			if(abilityState.Authority is Character)
			{
				AOEPrompt.Answer aoeAnswer =
					await PromptManager.Prompt(new AOEPrompt(abilityState, AOEPattern, TargetHex, null, () => "Select where to target"),
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
						new MonsterAOEPrompt(abilityState, AOEPattern, abilityState.AbilityRange, abilityState.AbilityRangeType, focus, null,
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
			if(customTargets != null)
			{
				figures.AddRange(customTargets);
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

				if(!Target.HasFlag(Target.Allies) && abilityState.Authority.AlliedWith(figure, false))
				{
					remove = true;
				}

				if(!Target.HasFlag(Target.Enemies) && abilityState.Authority.EnemiesWith(figure))
				{
					remove = true;
				}

				if(!Target.HasFlag(Target.Self) && abilityState.Performer == figure)
				{
					remove = true;
				}

				if(Target.HasFlag(Target.SelfCountsForTargets) && abilityState.SingleTargetStates.Count + 1 == abilityState.AbilityTargets &&
				   !abilityState.UniqueTargetedFigures.Contains(performer) && abilityState.Performer != figure)
				{
					remove = true;
				}

				if(!Target.HasFlag(Target.MustTargetSameWithAllTargets) && abilityState.UniqueTargetedFigures.Contains(figure))
				{
					remove = true;
				}

				if(Target.HasFlag(Target.MustTargetSameWithAllTargets) && abilityState.UniqueTargetedFigures.Count > 0 &&
				   abilityState.UniqueTargetedFigures[0] != figure)
				{
					remove = true;
				}

				if(Target.HasFlag(Target.MustTargetCharacters) && figure is not Character)
				{
					remove = true;
				}

				if(RequiresLineOfSight && !GameController.Instance.Map.HasLineOfSight(abilityState.Performer.Hex, figure.Hex))
				{
					remove = true;
				}

				ScenarioCheckEvents.CanBeTargetedCheck.Parameters canBeTargetedParameters =
					ScenarioCheckEvents.CanBeTargetedCheckEvent.Fire(new ScenarioCheckEvents.CanBeTargetedCheck.Parameters(performer, figure));

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
				bool autoSelectIfOne = Mandatory || (customTargets != null && customTargets.Count == 1) || (TargetHex != null && AOEPattern == null);
				TargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
					new TargetSelectionPrompt(getValidTargets, autoSelectIfOne, Mandatory, duringTargetedAbilityEffectCollection,
						() => GetTargetingHintText(abilityState)), abilityState.Authority);

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
						() => GetTargetingHintText(abilityState)), abilityState.Authority);

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
				await PushPull(abilityState, performer.Hex, target, abilityState.SingleTargetPull, false,
					() => $"Select a path to {Icons.HintText(Icons.Pull)}{abilityState.SingleTargetPull} target");
			}

			// Push
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetPush > 0)
			{
				await PushPull(abilityState, performer.Hex, target, abilityState.SingleTargetPush, true,
					() => $"Select a path to {Icons.HintText(Icons.Push)}{abilityState.SingleTargetPush} target");
			}

			await AfterEffects(abilityState, target);

			if(performer.IsDestroyed)
			{
				break;
			}

			if(customTargets != null && Target.HasFlag(Target.TargetAll))
			{
				if(abilityState.SingleTargetStates.Count == customTargets.Count)
				{
					break;
				}
			}
			else if(AOEPattern != null)
			{
				if(abilityState.TargetedHexes.Count == AOEPattern.Hexes.Count)
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

	protected async GDTask PushPull(T abilityState, Hex origin, Figure target, int distance, bool push, Func<string> hintText)
	{
		List<Vector2I> path = null;
		if(abilityState.Authority is Character)
		{
			PushPullPrompt.Answer pullAnswer = await PromptManager.Prompt(
				new PushPullPrompt(abilityState, origin, target, distance, push, null, hintText), abilityState.Authority);

			if(!pullAnswer.Skipped)
			{
				path = pullAnswer.Path;
			}
		}
		else
		{
			MonsterPushPullPrompt.Answer answer = await PromptManager.Prompt(
				new MonsterPushPullPrompt(abilityState, origin, target, distance, push, null, hintText), abilityState.Authority);

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

				await target.TweenGlobalPosition(hex.GlobalPosition, 0.2f).PlayFastForwardableAsync();
				await AbilityCmd.EnterHex(abilityState, target, abilityState.Authority, hex, true);
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