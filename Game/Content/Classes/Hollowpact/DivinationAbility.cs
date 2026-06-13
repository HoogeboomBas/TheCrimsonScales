using Fractural.Tasks;
using Godot;
using System.Collections.Generic;
using System.Linq;

public class DivinationAbility : TargetedAbility<DivinationAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
		public int CardsPlacedAtBottom = 0;
	}

	private int _cardsToPeek;
	private int _maxCardsToPlaceAtBottom;

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
			return (TBuilder)this;
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
			Obj.TargetType = _target ?? Target.SelfOrAllies;
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
	/// A convenience method that returns an instance of DivinationBuilder.
	/// </summary>
	/// <returns></returns>
	public static DivinationBuilder.ICardsToPeekStep Builder()
	{
		return new DivinationBuilder();
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		if(!GameController.FastForward)
		{
			GameController.Instance.VoidSightView.Open();

			await GDTask.DelayFastForwardable(2f);
		}

		ScenarioEvents.AMDCardPeekedEvent.Subscribe(abilityState, this,
			canApplyParameters => canApplyParameters.PotentialAbilityState == abilityState,
			async applyParameters =>
			{
				ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription placeAtDeckTopSubscription =
					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(applyFunction: async _ =>
						{
							applyParameters.SetPlaceAtDeckTop();

							await GDTask.CompletedTask;
						},
						effectType: EffectType.SelectableMandatory,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Triangle),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Place the card at the top of the deck."));

				ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription placeAtDeckBottomSubscription =
					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(applyFunction: async _ =>
						{
							applyParameters.SetPlaceAtDeckBottom();
							abilityState.CardsPlacedAtBottom++;

							await GDTask.CompletedTask;
						},
						effectType: EffectType.SelectableMandatory,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.UpsideDownTriangle),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Place the card at the bottom of the deck."));

				List<ScenarioEvents.GenericChoice.Subscription> subscriptions = [placeAtDeckTopSubscription];

				if(abilityState.CardsPlacedAtBottom < _maxCardsToPlaceAtBottom)
				{
					subscriptions.Add(placeAtDeckBottomSubscription);
				}

				await AbilityCmd.GenericChoice(abilityState.Authority, subscriptions);
			},
			effectType: EffectType.MandatoryBeforeOptionals
		);

		await GameController.Instance.AMDDrawView.PeekCards(abilityState, abilityState.Target.AMDCardDeck, _cardsToPeek);

		ScenarioEvents.AMDCardPeekedEvent.Unsubscribe(abilityState, this);

		if(!GameController.FastForward)
		{
			GameController.Instance.VoidSightView.Close();
		}
	}
}