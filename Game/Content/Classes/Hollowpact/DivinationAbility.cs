using Fractural.Tasks;
using Godot;

public class DivinationAbility : TargetedAbility<DivinationAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
		public int CardsPlacedAtBottom = 0;
	}

	protected int _cardsToPeek;
	protected int _maxCardsToPlaceAtBottom;

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in DivinationAbility. Enables inheritors of DivinationAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending DivinationAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.ICardsToPeekStep,
		AbstractBuilder<TBuilder, TAbility>.IMaxCardsToPlaceAtBottomStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : DivinationAbility, new()
	{
		public interface ICardsToPeekStep
		{
			IMaxCardsToPlaceAtBottomStep WithCardsToPeek(int cardsToPeek);
		}

		public interface IMaxCardsToPlaceAtBottomStep
		{
			TBuilder WithMaxCardsToPlaceAtBottom(int maxCardsToPlaceAtBottom);
		}

		public IMaxCardsToPlaceAtBottomStep WithCardsToPeek(int cardsToPeek)
		{
			Obj._cardsToPeek = cardsToPeek;
			return this;
		}

		public TBuilder WithMaxCardsToPlaceAtBottom(int maxCardsToPlaceAtBottom)
		{
			Obj._maxCardsToPlaceAtBottom = maxCardsToPlaceAtBottom;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.Target = _target ?? Target.SelfOrAllies;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class DivinationBuilder : AbstractBuilder<DivinationBuilder, DivinationAbility>
	{
		internal DivinationBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of ControlBuilder.
	/// </summary>
	/// <returns></returns>
	public static DivinationBuilder.ICardsToPeekStep Builder()
	{
		return new DivinationBuilder();
	}

	protected override async GDTask Perform(State abilityState)
	{
		ScenarioEvents.AMDCardPeekedEvent.Subscribe(abilityState, this,
			canApplyParameters => canApplyParameters.AbilityState == abilityState,
			async applyParameters =>
			{
				applyParameters.SetPlaceAtDeckBottom();
				abilityState.CardsPlacedAtBottom++;

				await GDTask.CompletedTask;
			},
			effectButtonParameters: new IconEffectButton.Parameters(Icons.EffectInfoViewTriangle),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"Place the card at the bottom of the deck."),
			effectType: EffectType.Selectable
		);

		for(int cardIndex = 0; cardIndex < _cardsToPeek; cardIndex++)
		{	
			await GameController.Instance.AMDDrawView.PeekCard(abilityState);

			if(abilityState.CardsPlacedAtBottom == _maxCardsToPlaceAtBottom)
			{
				break;
			}
		}

		ScenarioEvents.AMDCardPeekedEvent.Unsubscribe(abilityState, this);

		await GDTask.CompletedTask;

		//finish graphics
	}
}