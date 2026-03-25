using Fractural.Tasks;

public class VoidsightAbility : DivinationAbility
{
	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in VoidsightAbility. Enables inheritors of VoidsightAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending VoidsightAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : VoidsightAbility, new()
	{
		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj._cardsToPeek = 1;
			Obj._maxCardsToPlaceAtBottom = 1;
			Obj.Target = Target.Self;

			return base.Build();
		}
	}

	protected override async GDTask Perform(State abilityState)
	{
		await base.Perform(abilityState);
	}
}