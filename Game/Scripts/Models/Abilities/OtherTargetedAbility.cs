using System;
using Fractural.Tasks;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that does not neatly fit any other dedicated ability implementation.
/// </summary>
public class OtherTargetedAbility : TargetedAbility<OtherTargetedAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	private Func<State, Figure, GDTask> _onAfterConditionsApplied;

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

	protected override async GDTask AfterConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterConditionsApplied(abilityState, target);

		if(_onAfterConditionsApplied != null)
		{
			await _onAfterConditionsApplied(abilityState, target);
		}
	}
}