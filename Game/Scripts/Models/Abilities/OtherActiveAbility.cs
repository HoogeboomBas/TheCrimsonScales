using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// An <see cref="ActiveAbility{T}"/> that does not neatly fit any other dedicated ability implementation.
/// </summary>
public class OtherActiveAbility : ActiveAbility<OtherActiveAbility.State>
{
	public class State : ActiveAbilityState
	{
	}

	public Func<State, GDTask> OnActivate { get; protected set; }
	public Func<State, GDTask> OnDeactivate { get; protected set; }

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in OtherActiveAbility. Enables inheritors of OtherActiveAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending OtherActiveAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IOnActivateStep,
		AbstractBuilder<TBuilder, TAbility>.IOnDeactivateStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : OtherActiveAbility, new()
	{

		public interface IOnActivateStep
		{
			IOnDeactivateStep WithOnActivate(Func<State, GDTask> onActivate);
		}

		public interface IOnDeactivateStep
		{
			TBuilder WithOnDeactivate(Func<State, GDTask> onDeactivate);
		}

		public IOnDeactivateStep WithOnActivate(Func<State, GDTask> onActivate)
		{
			Obj.OnActivate = onActivate;
			return (TBuilder)this;
		}

		public TBuilder WithOnDeactivate(Func<State, GDTask> onDeactivate)
		{
			Obj.OnDeactivate = onDeactivate;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class OtherActiveBuilder : AbstractBuilder<OtherActiveBuilder, OtherActiveAbility>
	{
		internal OtherActiveBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of OtherActiveBuilder.
	/// </summary>
	/// <returns></returns>
	public static AbstractBuilder<OtherActiveBuilder, OtherActiveAbility>.IOnActivateStep Builder()
	{
		return new OtherActiveBuilder();
	}

	public OtherActiveAbility() { }

	public OtherActiveAbility(Func<State, GDTask> onActivate, Func<State, GDTask> onDeactivate,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getHintText = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed, conditionalAbilityCheck, getHintText, abilityStartedSubscriptions,
			abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		OnActivate = onActivate;
		OnDeactivate = onDeactivate;
	}

	protected override async GDTask Perform(State abilityState)
	{
		await AskConfirmAndActivate(abilityState);
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);

		await OnActivate(abilityState);
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		await OnDeactivate(abilityState);
	}
}