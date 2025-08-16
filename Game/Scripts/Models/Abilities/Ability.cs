using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// An Ability is a typical GH Ability. They can be performed using an Action State, or just using a Figure performer.
/// </summary>
public abstract class Ability<T> : Ability
	where T : AbilityState, new()
{
	public delegate GDTask<bool> ConditionalAbilityCheckDelegate(T abilityState);

	private readonly Func<T, GDTask> _onAbilityStarted;
	private readonly Func<T, GDTask> _onAbilityEnded;
	private readonly Func<T, GDTask> _onAbilityEndedPerformed;

	private readonly ConditionalAbilityCheckDelegate _conditionalAbilityCheck;
	
	public bool PerformWhileStunned { get; set; } = false;

	public List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> AbilityStartedSubscriptions { get; }
	public List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> AbilityEndedSubscriptions { get; }
	public List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> AbilityPerformedSubscriptions { get; }

	protected Ability(Func<T, GDTask> onAbilityStarted, Func<T, GDTask> onAbilityEnded, Func<T, GDTask> onAbilityEndedPerformed,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions)
	{
		_onAbilityStarted = onAbilityStarted;
		_onAbilityEnded = onAbilityEnded;
		_onAbilityEndedPerformed = onAbilityEndedPerformed;

		_conditionalAbilityCheck = conditionalAbilityCheck;

		AbilityStartedSubscriptions = abilityStartedSubscriptions;
		AbilityEndedSubscriptions = abilityEndedSubscriptions;
		AbilityPerformedSubscriptions = abilityPerformedSubscriptions;
	}

	public override async GDTask Perform(ActionState actionState)
	{
		T abilityState = new T()
		{
			PerformWhileStunned = PerformWhileStunned,
			ActionState = actionState
		};

		InitializeState(abilityState);

		actionState.AddAbilityState(abilityState);

		await StartPerform(abilityState);

		if(!abilityState.Blocked)
		{
			await Perform(abilityState);
		}

		await EndPerform(abilityState);
	}

	protected virtual void InitializeState(T abilityState)
	{
	}

	protected virtual async GDTask StartPerform(T abilityState)
	{
		if(_onAbilityStarted != null)
		{
			await _onAbilityStarted(abilityState);
		}

		if(_conditionalAbilityCheck != null)
		{
			if(!await _conditionalAbilityCheck(abilityState))
			{
				abilityState.SetBlocked();
				return;
			}
		}

		ScenarioEvents.AbilityStartedEvent.Subscribe(abilityState, this, AbilityStartedSubscriptions);
		ScenarioEvents.AbilityStarted.Parameters abilityStartedParameters =
			await ScenarioEvents.AbilityStartedEvent.CreatePrompt(
				new ScenarioEvents.AbilityStarted.Parameters(abilityState));
		if(abilityStartedParameters.IsBlocked)
		{
			abilityState.SetBlocked();
		}

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(AbilityStartedSubscriptions);
	}

	protected abstract GDTask Perform(T abilityState);

	protected virtual async GDTask EndPerform(T abilityState)
	{
		if(abilityState.Performed && !abilityState.Blocked)
		{
			ScenarioEvents.AbilityPerformedEvent.Subscribe(abilityState, this, AbilityPerformedSubscriptions);
			ScenarioEvents.AbilityPerformed.Parameters abilityPerformedParameters =
				await ScenarioEvents.AbilityPerformedEvent.CreatePrompt(
					new ScenarioEvents.AbilityPerformed.Parameters(abilityState));
			ScenarioEvents.AbilityPerformedEvent.Unsubscribe(AbilityPerformedSubscriptions);

			if(_onAbilityEndedPerformed != null)
			{
				await _onAbilityEndedPerformed(abilityState);
			}
		}

		ScenarioEvents.AbilityEndedEvent.Subscribe(abilityState, this, AbilityEndedSubscriptions);
		ScenarioEvents.AbilityEnded.Parameters abilityEndedParameters =
			await ScenarioEvents.AbilityEndedEvent.CreatePrompt(
				new ScenarioEvents.AbilityEnded.Parameters(abilityState));
		ScenarioEvents.AbilityEndedEvent.Unsubscribe(AbilityEndedSubscriptions);

		if(_onAbilityEnded != null)
		{
			await _onAbilityEnded(abilityState);
		}
	}
}

public abstract class Ability
{
	// public GDTask Perform(ActionState actionState)
	// {
	// 	return Perform(actionState, actionState.PerformerAndAuthority, actionState.PerformerAndAuthority);
	// }

	public abstract GDTask Perform(ActionState actionState);
}