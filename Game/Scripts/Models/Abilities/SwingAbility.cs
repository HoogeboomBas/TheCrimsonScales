/// <summary>
/// A forced movement <see cref="TargetedAbility{T, TSingleTargetState}"/> that moves the enemy in a circular motion around the acting figure,
/// ignoring most movement rules.
/// </summary>
public class SwingAbility : TargetedAbility<SwingAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in SwingAbility. Enables inheritors of SwingAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending SwingAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.ISwingStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : SwingAbility, new()
	{
		public interface ISwingStep
		{
			TBuilder WithSwing(int swing);
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class SwingBuilder : AbstractBuilder<SwingBuilder, SwingAbility>
	{
		internal SwingBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of SwingBuilder.
	/// </summary>
	/// <returns></returns>
	public static SwingBuilder.ISwingStep Builder()
	{
		return new SwingBuilder();
	}

	public SwingAbility() { }
}