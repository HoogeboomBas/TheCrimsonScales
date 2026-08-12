using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

/// <summary>
/// An <see cref="Ability{T}"/> that makes figures suffer damage.
/// </summary>
public class SufferDamageAbility : Ability<SufferDamageAbility.State>
{
	public class State : AbilityState, IAOEAbilityState
	{
		public List<Figure> TargetedFigures { get; } = new List<Figure>();
		public List<Figure> UniqueTargetedFigures { get; } = new List<Figure>();
		public List<Hex> TargetedHexes { get; } = new List<Hex>();
		public List<AOEHex> TargetedAOEHexes { get; set; }

		public Target AbilityTarget { get; set; }
		public int AbilityTargets { get; set; }
		public Action<State, List<Figure>> AbilityCustomGetTargets { get; set; }
		public AOEPattern AbilityAOEPattern { get; set; }
		public int AbilityRange { get; set; }

		public int AbilityDamage { get; set; }

		public int SingleTargetDamage { get; set; }

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
	}

	private static readonly List<Hex> HexCache = new List<Hex>();

	private Func<State, string> _getTargetingHintText;

	public DynamicInt<State> Damage { get; protected set; }
	public int Range { get; private set; } = 1;
	public bool RequiresLineOfSight { get; private set; } = false;

	public Target Target { get; protected set; } = Target.Enemies;
	public int Targets { get; private set; } = 1;
	public Hex TargetHex { get; private set; }
	public AOEPattern AOEPattern { get; private set; }
	public bool Mandatory { get; private set; }

	public Action<State, List<Figure>> CustomGetTargets { get; private set; }

	public bool IsMultiTarget =>
		Targets > 1 ||
		Target.HasFlag(Target.TargetAll) ||
		(AOEPattern != null && AOEPattern.LocalHexes.Count(hex => hex.Type == AOEHexType.Red) > 1);

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in TargetedAbility. Enables inheritors of TargetedAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending TargetedAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IDamageStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : SufferDamageAbility, new()
	{
		protected Target? _target;
		protected bool? _mandatory;
		protected Func<State, string> GetTargetingHintText;

		public interface IDamageStep
		{
			TBuilder WithDamage(DynamicInt<State> damage);
		}

		public TBuilder WithDamage(DynamicInt<State> damage)
		{
			Obj.Damage = damage;
			return (TBuilder)this;
		}

		public TBuilder WithGetTargetingHintText(Func<State, string> getTargetingHintText)
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
			_mandatory = mandatory;
			Obj.Mandatory = mandatory;
			return (TBuilder)this;
		}

		public TBuilder WithCustomGetTargets(Action<State, List<Figure>> getTargets)
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
			Obj.Mandatory = _mandatory ?? (Obj.Target.HasFlag(Target.Self));
			Obj._getTargetingHintText = GetTargetingHintText ?? Obj.DefaultTargetingHintText;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class SufferDamageBuilder : AbstractBuilder<SufferDamageBuilder, SufferDamageAbility>
	{
		internal SufferDamageBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of AttackBuilder.
	/// </summary>
	/// <returns></returns>
	public static SufferDamageBuilder.IDamageStep Builder()
	{
		return new SufferDamageBuilder();
	}

	public SufferDamageAbility() { }

	protected override void InitializeState(State abilityState)
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
		abilityState.AbilityCustomGetTargets = CustomGetTargets != null
			? (state, figures) => CustomGetTargets(state, figures)
			: null;

		abilityState.AbilityDamage = Damage.GetValue(abilityState);
	}

	protected override async GDTask Perform(State abilityState)
	{
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
				Figure focus = (await abilityState.ActionState.GetFocus(abilityState)).Item1;

				MonsterAOEPrompt.Answer aoeAnswer =
					await PromptManager.Prompt(
						new MonsterAOEPrompt(abilityState, abilityState.AbilityAOEPattern, abilityState.AbilityRange, RangeType.Melee,
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

			Figure target;

			if(abilityState.Authority is Character)
			{
				bool autoSelectIfOne =
					Mandatory ||
					abilityState.AbilityTarget == Target.Self ||
					(TargetHex != null && abilityState.AbilityAOEPattern == null);
				target = await AbilityCmd.SelectFigure(abilityState, getValidTargets, mandatory: Mandatory,
					autoSelectIfOne: autoSelectIfOne, autoSkipIfNone: true,
					effectCollection: null,
					hintText: () => _getTargetingHintText(abilityState));
			}
			else
			{
				Figure focus = (await abilityState.ActionState.GetFocus(abilityState)).Item1;

				bool focusWasTargeted = abilityState.UniqueTargetedFigures.Contains(focus);

				MonsterTargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
					new MonsterTargetSelectionPrompt(getValidTargets, true, focus, focusWasTargeted, null,
						() => _getTargetingHintText(abilityState)), abilityState.Authority);

				target = targetAnswer.Skipped ? null : GameController.Instance.ReferenceManager.Get<Figure>(targetAnswer.FigureReferenceId);
			}

			if(target == null)
			{
				break;
			}

			abilityState.TargetedFigures.Add(target);
			abilityState.UniqueTargetedFigures.AddIfNew(target);
			abilityState.TargetedHexes.AddIfNew(target.Hex);
			if(!abilityState.GetRedAOEHexes().Contains(target.Hex))
			{
				targetsOutOfAOE++;
			}

			abilityState.SetPerformed();

			await AbilityCmd.SufferDamage(abilityState, target, abilityState.SingleTargetDamage);

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
			else if(abilityState.TargetedFigures.Count == abilityState.AbilityTargets)
			{
				break;
			}
		}
	}

	private void InitAbilityStateForSingleTarget(State abilityState)
	{
		abilityState.SingleTargetDamage = abilityState.AbilityDamage;
	}

	private void GetValidTargets(State abilityState, List<Figure> figures, int targetsOutOfAOE)
	{
		Figure performer = abilityState.Performer;

		if(abilityState.AbilityTarget == Target.Self)
		{
			figures.Add(performer);
		}
		else if(abilityState.AbilityCustomGetTargets != null)
		{
			CustomGetTargets(abilityState, figures);
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
				RangeHelper.FindHexesInRange(performer.Hex, abilityState.AbilityRange, true, HexCache);

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
			RangeHelper.FindHexesInRange(performer.Hex, abilityState.AbilityRange, true, HexCache);

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
			   //abilityState.SingleTargetStates.Count + 1 == abilityState.AbilityTargets &&
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

			if(RequiresLineOfSight && !GameController.Instance.Map.HasLineOfSight(performer.Hex, figure.Hex))
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
	}

	private string DefaultTargetingHintText(State abilityState)
	{
		return $"Select a target to suffer {Icons.HintText(Icons.Damage)}{Damage.GetValue(abilityState)}";
	}
}