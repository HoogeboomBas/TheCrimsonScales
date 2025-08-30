using System;
using Fractural.Tasks;

/// <summary>
/// An <see cref="Ability{T}"/> that does not neatly fit any other dedicated ability implementation.
/// </summary>
public class OtherAbility : Ability<OtherAbility.State>
{
	public class State : AbilityState
	{
	}

	private Func<State, GDTask> _performAbility { get; set; }

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in OtherAbility. Enables inheritors of OtherAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending OtherAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IPerformAbilityStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : OtherAbility, new()
	{
		public interface IPerformAbilityStep
		{
			TBuilder WithPerformAbility(Func<State, GDTask> performAbility);
		}

		public TBuilder WithPerformAbility(Func<State, GDTask> performAbility)
		{
			Obj._performAbility = performAbility;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class OtherBuilder : AbstractBuilder<OtherBuilder, OtherAbility>
	{
		internal OtherBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of OtherAbilityBuilder.
	/// </summary>
	/// <returns></returns>
	public static OtherBuilder.IPerformAbilityStep Builder()
	{
		return new OtherBuilder();
	}

	public OtherAbility() { }

	protected override async GDTask Perform(State abilityState)
	{
		await _performAbility(abilityState);
	}
}