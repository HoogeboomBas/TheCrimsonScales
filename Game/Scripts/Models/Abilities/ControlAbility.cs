using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that allows a figure to control abilities to other figures.
/// </summary>
public class ControlAbility : TargetedAbility<ControlAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	private Func<State, List<Ability>> _getAbilities;

	public List<ScenarioEvents.DuringControl.Subscription> DuringControlSubscriptions { get; private set; } = [];

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in ControlAbility. Enables inheritors of ControlAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending ControlAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IGetAbilitiesStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : ControlAbility, new()
	{
		public interface IGetAbilitiesStep
		{
			TBuilder WithGetAbilities(Func<State, List<Ability>> getAbilities);
		}

		public TBuilder WithGetAbilities(Func<State, List<Ability>> getAbilities)
		{
			Obj._getAbilities = getAbilities;
			return (TBuilder)this;
		}

		public TBuilder WithDuringControlSubscription(ScenarioEvents.DuringControl.Subscription duringControlSubscription)
		{
			Obj.DuringControlSubscriptions.Add(duringControlSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithDuringControlSubscriptions(List<ScenarioEvents.DuringControl.Subscription> duringControlSubscriptions)
		{
			Obj.DuringControlSubscriptions = duringControlSubscriptions;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.Target = _target ?? Target.Enemies;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class ControlBuilder : AbstractBuilder<ControlBuilder, ControlAbility>
	{
		internal ControlBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of ControlBuilder.
	/// </summary>
	/// <returns></returns>
	public static ControlBuilder.IGetAbilitiesStep Builder()
	{
		return new ControlBuilder();
	}

	public ControlAbility() { }

	protected override async GDTask StartPerform(State abilityState)
	{
		await base.StartPerform(abilityState);

		ScenarioEvents.DuringControlEvent.Subscribe(abilityState, this, DuringControlSubscriptions);
	}

	protected override async GDTask EndPerform(State abilityState)
	{
		await base.EndPerform(abilityState);

		ScenarioEvents.DuringControlEvent.Unsubscribe(DuringControlSubscriptions);
	}

	protected override EffectCollection CreateDuringTargetedAbilityEffectCollection(State abilityState)
	{
		return ScenarioEvents.DuringControlEvent.CreateEffectCollection(new ScenarioEvents.DuringControl.Parameters(abilityState));
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);

		// Perform the actual abilities
		ActionState actionState = new ActionState(target, target is Character ? target : abilityState.Performer, _getAbilities(abilityState),
			abilityState.ActionState);
		await actionState.Perform();
	}
}