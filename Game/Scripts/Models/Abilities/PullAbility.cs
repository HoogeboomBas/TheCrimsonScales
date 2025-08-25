using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A forced movement <see cref="TargetedAbility{T, TSingleTargetState}"/> that moves the enemy towards the acting figure,
/// ignoring most movement rules.
/// </summary>
public class PullAbility : TargetedAbility<PullAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}
	
	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in PullAbility. Enables inheritors of PullAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending PullAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IPullStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : PullAbility, new()
	{
		public interface IPullStep
		{
			TBuilder WithPull(int push);
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class PullBuilder : AbstractBuilder<PullBuilder, PullAbility>
	{
		internal PullBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of PullBuilder.
	/// </summary>
	/// <returns></returns>
	public static PullBuilder.IPullStep Builder()
	{
		return new PullBuilder();
	}

	public PullAbility() { }

	public PullAbility(int pull, int targets = 1, int? range = null, RangeType? rangeType = null,
		Target target = Target.Enemies,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, int push = 0, ConditionModel[] conditions = null,
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
	}
}