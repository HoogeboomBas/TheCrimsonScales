using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A forced movement <see cref="TargetedAbility{T, TSingleTargetState}"/> that moves the acting figure towards the target,
/// ignoring most movement rules.
/// </summary>
public class PullSelfAbility : TargetedAbility<PullSelfAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	public int PullSelfValue { get; protected set; }
	
	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in PullSelfAbility. Enables inheritors of PullSelfAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending PullSelfAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IPullPullSelfStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : PullSelfAbility, new()
	{
		public interface IPullPullSelfStep
		{
			TBuilder WithPullSelfValue(int pullSelfValue);
		}
		
		public TBuilder WithPullSelfValue(int pullSelfValue)
		{
			Obj.PullSelfValue = pullSelfValue;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class PullBuilder : AbstractBuilder<PullBuilder, PullSelfAbility>
	{
		internal PullBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of PullBuilder.
	/// </summary>
	/// <returns></returns>
	public static PullBuilder.IPullPullSelfStep Builder()
	{
		return new PullBuilder();
	}

	public PullSelfAbility() { }

	public PullSelfAbility(int pull, int targets = 1, int? range = null, RangeType? rangeType = null,
		Target target = Target.Enemies,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, ConditionModel[] conditions = null,
		Action<State, List<Figure>> customGetTargets = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getTargetingHintText = null,
		List<ScenarioEvents.AbilityStarted.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvents.AbilityEnded.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(targets, range, rangeType, target,
			requiresLineOfSight, mandatory, targetHex, aoePattern, 0, 0, conditions,
			customGetTargets, onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed,
			conditionalAbilityCheck, getTargetingHintText, abilityStartedSubscriptions, abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		PullSelfValue = pull;
	}

	protected override async GDTask AfterConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterConditionsApplied(abilityState, target);

		await PushPull(abilityState, target.Hex, abilityState.Performer, PullSelfValue, false, () => $"Select a path to {Icons.HintText(Icons.Pull)}{PullSelfValue} self toward target");
	}
}