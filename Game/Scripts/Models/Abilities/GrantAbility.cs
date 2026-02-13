using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that allows a figure to grant abilities to other figures.
/// </summary>
public class GrantAbility : TargetedAbility<GrantAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
		public List<ActionState> GrantAbilityActionStates { get; } = new List<ActionState>();
	}

	private List<Ability> _abilities;
	private Func<State, List<Ability>> _getAbilities;

	public List<ScenarioEvents.DuringGrant.Subscription> DuringGrantSubscriptions { get; private set; } = [];

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in GrantAbility. Enables inheritors of GrantAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending GrantAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IGetAbilitiesStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : GrantAbility, new()
	{
		public interface IGetAbilitiesStep
		{
			TBuilder WithAbilities(List<Ability> abilities);
			TBuilder WithGetAbilities(Func<State, List<Ability>> getAbilities);
		}

		public TBuilder WithAbilities(List<Ability> abilities)
		{
			Obj._abilities = abilities;
			return (TBuilder)this;
		}

		public TBuilder WithGetAbilities(Func<State, List<Ability>> getAbilities)
		{
			Obj._getAbilities = getAbilities;
			return (TBuilder)this;
		}

		public TBuilder WithDuringGrantSubscription(ScenarioEvents.DuringGrant.Subscription duringGrantSubscription)
		{
			Obj.DuringGrantSubscriptions.Add(duringGrantSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithDuringGrantSubscriptions(List<ScenarioEvents.DuringGrant.Subscription> duringGrantSubscriptions)
		{
			Obj.DuringGrantSubscriptions.AddRange(duringGrantSubscriptions);
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			_target ??= Target.Allies;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class GrantBuilder : AbstractBuilder<GrantBuilder, GrantAbility>
	{
		internal GrantBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of GrantBuilder.
	/// </summary>
	/// <returns></returns>
	public static GrantBuilder.IGetAbilitiesStep Builder()
	{
		return new GrantBuilder();
	}

	public GrantAbility() { }

	protected override async GDTask StartPerform(State abilityState)
	{
		await base.StartPerform(abilityState);

		ScenarioEvents.DuringGrantEvent.Subscribe(abilityState, this, DuringGrantSubscriptions);
	}

	protected override async GDTask EndPerform(State abilityState)
	{
		await base.EndPerform(abilityState);

		ScenarioEvents.DuringGrantEvent.Unsubscribe(DuringGrantSubscriptions);
	}

	protected override EffectCollection CreateDuringTargetedAbilityEffectCollection(State abilityState)
	{
		return ScenarioEvents.DuringGrantEvent.CreateEffectCollection(new ScenarioEvents.DuringGrant.Parameters(abilityState));
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);
		// Perform the actual abilities
		ActionState actionState = new ActionState(abilityState.ActionState.ActionSource, target,
			target is Character ? target : abilityState.Performer, _abilities ?? _getAbilities(abilityState),
			abilityState.ActionState);
		abilityState.GrantAbilityActionStates.Add(actionState);

		await actionState.Perform();
	}

	protected override string DefaultTargetingHintText(State abilityState)
	{
		return "Select a target for the grant ability";
	}
}