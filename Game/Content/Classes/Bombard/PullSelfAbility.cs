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

	public int PullSelfValue { get; private set; }

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
	public class PullSelfBuilder : AbstractBuilder<PullSelfBuilder, PullSelfAbility>
	{
		internal PullSelfBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of PullBuilder.
	/// </summary>
	/// <returns></returns>
	public static PullSelfBuilder.IPullPullSelfStep Builder()
	{
		return new PullSelfBuilder();
	}

	public PullSelfAbility() { }

	protected override async GDTask AfterConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterConditionsApplied(abilityState, target);

		await ForcedMovement(abilityState, target.Hex, abilityState.Performer, PullSelfValue, ForcedMovementType.Pull,
			() => $"Select a path to {Icons.HintText(Icons.Pull)}{PullSelfValue} self toward target");
	}
}