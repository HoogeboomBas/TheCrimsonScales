using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that applies at least one condition to a figure.
/// </summary>
public class ConditionAbility : TargetedAbility<ConditionAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	public List<ScenarioEvent<ScenarioEvents.ConditionAfterTargetConfirmed.Parameters>.Subscription>
		AfterTargetConfirmedSubscriptions { get; protected set; } = [];

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in ConditionAbility Enables inheritors of ConditionAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending ConditionAbility
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IConditionsStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : ConditionAbility, new()
	{
		public interface IConditionsStep
		{
			TBuilder WithConditions(params ConditionModel[] conditions);
		}

		public TBuilder WithConditions(params ConditionModel[] conditions)
		{
			Obj.Conditions = conditions;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			// TODO varadski 21.08.2025: I would maybe rather throw an exception if there are no conditions; should be mandatory for ConditionAbility
			Obj.Target = _target ?? ((Obj.Conditions.Length > 0 && Obj.Conditions[0].IsPositive) ? Target.SelfOrAllies : Target.Enemies);
			return base.Build();
		}

	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class ConditionBuilder : AbstractBuilder<ConditionBuilder, ConditionAbility>
	{
		internal ConditionBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of ConditionBuilder.
	/// </summary>
	/// <returns></returns>
	public static ConditionBuilder.IConditionsStep Builder()
	{
		return new ConditionBuilder();
	}

	public ConditionAbility() { }

	public ConditionAbility(ConditionModel[] conditions, int targets = 1, int? range = null, RangeType? rangeType = null,
		Target? target = null,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, int push = 0, int pull = 0,
		Action<State, List<Figure>> customGetTargets = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getTargetingHintText = null,
		List<ScenarioEvents.ConditionAfterTargetConfirmed.Subscription> afterTargetConfirmedSubscriptions = null,
		List<ScenarioEvents.AbilityStarted.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvents.AbilityEnded.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(targets, range, rangeType, target ?? ((conditions.Length > 0 && conditions[0].IsPositive) ? Target.SelfOrAllies : Target.Enemies),
			requiresLineOfSight, mandatory, targetHex, aoePattern, push, pull, conditions,
			customGetTargets, onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed,
			conditionalAbilityCheck, getTargetingHintText, abilityStartedSubscriptions, abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		AfterTargetConfirmedSubscriptions = afterTargetConfirmedSubscriptions;
	}

	protected override async GDTask StartPerform(State abilityState)
	{
		await base.StartPerform(abilityState);

		ScenarioEvents.ConditionAfterTargetConfirmedEvent.Subscribe(abilityState, this, AfterTargetConfirmedSubscriptions);
	}

	protected override async GDTask EndPerform(State abilityState)
	{
		await base.EndPerform(abilityState);

		ScenarioEvents.ConditionAfterTargetConfirmedEvent.Unsubscribe(AfterTargetConfirmedSubscriptions);
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await ScenarioEvents.ConditionAfterTargetConfirmedEvent.CreatePrompt(
			new ScenarioEvents.ConditionAfterTargetConfirmed.Parameters(abilityState), abilityState);
	}

	protected override string DefaultTargetingHintText(State abilityState)
	{
		string conditonIconsText = string.Empty;
		foreach(ConditionModel conditionModel in Conditions)
		{
			conditonIconsText += Icons.HintText(Icons.GetCondition(conditionModel));
		}

		if(Target == Target.Self)
		{
			return $"Perform {conditonIconsText} self?";
		}

		return $"Select a target for {conditonIconsText}";
	}
}