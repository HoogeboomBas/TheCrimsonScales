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

	private Func<T, GDTask> _onAbilityStarted { get; set; }
	private Func<T, GDTask> _onAbilityEnded { get; set; }
	private Func<T, GDTask> _onAbilityEndedPerformed { get; set; }

	private ConditionalAbilityCheckDelegate ConditionalAbilityCheck { get; set; }

	public List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> AbilityStartedSubscriptions { get; protected set; } = [];
	public List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> AbilityEndedSubscriptions { get; protected set; } = [];
	public List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> AbilityPerformedSubscriptions { get; protected set; } = [];

	/// <summary>
	/// A builder utilizing generics, which enables inheritors to extend the builder as well.
	/// Simplifies adding new properties, which will automatically be propagated to inheritor builders.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any Ability extending Ability.
	public abstract class AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : Ability<T>, new()
	{
		protected readonly TAbility Obj = new TAbility();

		public TBuilder WithOnAbilityStarted(Func<T, GDTask> onAbilityStarted)
		{
			Obj._onAbilityStarted = onAbilityStarted;
			return (TBuilder)this;
		}

		public TBuilder WithOnAbilityEnded(Func<T, GDTask> onAbilityEnded)
		{
			Obj._onAbilityEnded = onAbilityEnded;
			return (TBuilder)this;
		}

		public TBuilder WithOnAbilityEndedPerformed(Func<T, GDTask> onAbilityEndedPerformed)
		{
			Obj._onAbilityEndedPerformed = onAbilityEndedPerformed;
			return (TBuilder)this;
		}

		public TBuilder WithConditionalAbilityCheck(ConditionalAbilityCheckDelegate conditionalAbilityCheck)
		{
			Obj.ConditionalAbilityCheck = conditionalAbilityCheck;
			return (TBuilder)this;
		}

		public TBuilder WithAbilityStartedSubscription(
			ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription abilityStartedSubscription)
		{
			Obj.AbilityStartedSubscriptions.Add(abilityStartedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAbilityStartedSubscriptions(
			List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions)
		{
			Obj.AbilityStartedSubscriptions = abilityStartedSubscriptions;
			return (TBuilder)this;
		}

		public TBuilder WithAbilityEndedSubscription(ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription abilityEndedSubscription)
		{
			Obj.AbilityEndedSubscriptions.Add(abilityEndedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAbilityEndedSubscriptions(
			List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions)
		{
			Obj.AbilityEndedSubscriptions = abilityEndedSubscriptions;
			return (TBuilder)this;
		}

		public TBuilder WithAbilityPerformedSubscription(
			ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription abilityPerformedSubscription)
		{
			Obj.AbilityPerformedSubscriptions.Add(abilityPerformedSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithAbilityPerformedSubscriptions(
			List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions)
		{
			Obj.AbilityPerformedSubscriptions = abilityPerformedSubscriptions;
			return (TBuilder)this;
		}

		public virtual TAbility Build()
		{
			return Obj;
		}
	}

	protected Ability() { }

	protected Ability(Func<T, GDTask> onAbilityStarted, Func<T, GDTask> onAbilityEnded,
		Func<T, GDTask> onAbilityEndedPerformed,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions)
	{
		_onAbilityStarted = onAbilityStarted;
		_onAbilityEnded = onAbilityEnded;
		_onAbilityEndedPerformed = onAbilityEndedPerformed;

		ConditionalAbilityCheck = conditionalAbilityCheck;

		AbilityStartedSubscriptions = abilityStartedSubscriptions;
		AbilityEndedSubscriptions = abilityEndedSubscriptions;
		AbilityPerformedSubscriptions = abilityPerformedSubscriptions;
	}

	public override async GDTask Perform(ActionState actionState)
	{
		T abilityState = new T()
		{
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

		if(ConditionalAbilityCheck != null)
		{
			if(!await ConditionalAbilityCheck(abilityState))
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