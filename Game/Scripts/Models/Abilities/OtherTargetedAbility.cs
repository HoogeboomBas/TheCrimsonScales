using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that does not neatly fit any other dedicated ability implementation.
/// </summary>
public class OtherTargetedAbility : TargetedAbility<OtherTargetedAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	protected Func<State, Figure, GDTask> _onAfterConditionsApplied { get; set; }
	
	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in OtherTargetedAbility. Enables inheritors of OtherTargetedAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending OtherTargetedAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : OtherTargetedAbility, new()
	{
		public TBuilder WithOnAfterConditionsApplied(Func<State, Figure, GDTask> onAfterConditionsApplied)
		{
			Obj._onAfterConditionsApplied = onAfterConditionsApplied;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.Target = _target ?? Target.Allies;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class OtherTargetedBuilder : AbstractBuilder<OtherTargetedBuilder, OtherTargetedAbility>
	{
		internal OtherTargetedBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of OtherTargetedBuilder.
	/// </summary>
	/// <returns></returns>
	public static OtherTargetedBuilder Builder()
	{
		return new OtherTargetedBuilder();
	}

	public OtherTargetedAbility() { }

	public OtherTargetedAbility(Func<State, Figure, GDTask> onAfterConditionsApplied = null, int targets = 1, int? range = null, RangeType? rangeType = null,
		Target target = Target.Allies,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, int push = 0, int pull = 0, ConditionModel[] conditions = null,
		Action<State, List<Figure>> customGetTargets = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getTargetingHintText = null,
		List<ScenarioEvents.AbilityStarted.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvents.AbilityEnded.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(targets, range, rangeType, target,
			requiresLineOfSight, mandatory, targetHex, aoePattern, push, pull, conditions,
			customGetTargets, onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed,
			conditionalAbilityCheck, getTargetingHintText, abilityStartedSubscriptions, abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		_onAfterConditionsApplied = onAfterConditionsApplied;
	}

	protected override async GDTask AfterConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterConditionsApplied(abilityState, target);

		if(_onAfterConditionsApplied != null)
		{
			await _onAfterConditionsApplied(abilityState, target);
		}
	}
}