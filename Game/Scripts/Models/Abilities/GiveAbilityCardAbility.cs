using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that gives an ability card to another character.
/// </summary>
public class GiveAbilityCardAbility : TargetedAbility<GiveAbilityCardAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	private Action<AbilityState, List<AbilityCard>> _getAbilityCards;
	private Func<AbilityState, AbilityCard, GDTask> _onCardGiven;
	private Func<AbilityCard, GDTask> _onCardDiscarded;
	private Func<AbilityCard, GDTask> _onCardLost;

	private bool _selectAutomatically;

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in GiveAbilityCardAbility. Enables inheritors of GiveAbilityCardAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending GiveAbilityCardAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IGetAbilityCardsStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : GiveAbilityCardAbility, new()
	{
		public interface IGetAbilityCardsStep
		{
			TBuilder WithGetAbilityCards(Action<AbilityState, List<AbilityCard>> getAbilityCards);
		}

		public TBuilder WithGetAbilityCards(Action<AbilityState, List<AbilityCard>> getAbilityCards)
		{
			Obj._getAbilityCards = getAbilityCards;
			return (TBuilder)this;
		}

		public TBuilder WithOnCardGiven(Func<AbilityState, AbilityCard, GDTask> onCardGiven)
		{
			Obj._onCardGiven = onCardGiven;
			return (TBuilder)this;
		}

		public TBuilder WithOnCardDiscarded(Func<AbilityCard, GDTask> onCardDiscarded)
		{
			Obj._onCardDiscarded = onCardDiscarded;
			return (TBuilder)this;
		}

		public TBuilder WithOnCardLost(Func<AbilityCard, GDTask> onCardLost)
		{
			Obj._onCardLost = onCardLost;
			return (TBuilder)this;
		}

		public TBuilder WithSelectAutomatically(bool selectAutomatically)
		{
			Obj._selectAutomatically = selectAutomatically;
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
	public class GiveAbilityCardBuilder : AbstractBuilder<GiveAbilityCardBuilder, GiveAbilityCardAbility>
	{
		internal GiveAbilityCardBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of GiveAbilityCardBuilder.
	/// </summary>
	/// <returns></returns>
	public static GiveAbilityCardBuilder.IGetAbilityCardsStep Builder()
	{
		return new GiveAbilityCardBuilder();
	}

	public GiveAbilityCardAbility() { }

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);

		await GiveAbilityCard(abilityState, target, _getAbilityCards, _onCardGiven, _onCardDiscarded, _onCardLost, _selectAutomatically);
	}

	public static async GDTask GiveAbilityCard(AbilityState abilityState, Figure target, Action<AbilityState, List<AbilityCard>> getAbilityCards,
		Func<AbilityState, AbilityCard, GDTask> onCardGiven, Func<AbilityCard, GDTask> onCardDiscarded, Func<AbilityCard, GDTask> onCardLost,
		bool selectAutomatically = false)
	{
		AbilityCard abilityCard;
		if(selectAutomatically)
		{
			List<AbilityCard> abilityCards = new List<AbilityCard>();
			getAbilityCards(abilityState, abilityCards);
			abilityCard = abilityCards.Count == 0 ? null : abilityCards[0];
		}
		else
		{
			abilityCard = await AbilityCmd.SelectAbilityCard(abilityState.Authority, list => getAbilityCards(abilityState, list), CardState.Hand);
		}

		if(abilityCard != null && target is Character character)
		{
			if(onCardGiven != null)
			{
				await onCardGiven(abilityState, abilityCard);
			}

			character.AddCard(abilityCard);

			object subscriber = new object();
			ScenarioEvents.AbilityCardStateChangedEvent.Subscribe(abilityState, subscriber,
				parameters => parameters.AbilityCard == abilityCard,
				async parameters =>
				{
					if(abilityCard.CardState == CardState.Discarded)
					{
						if(onCardDiscarded != null)
						{
							ScenarioEvents.AbilityCardStateChangedEvent.Unsubscribe(abilityState, subscriber);

							await onCardDiscarded(abilityCard);
						}
					}
					else if(abilityCard.CardState == CardState.Lost || abilityCard.CardState == CardState.UnrecoverablyLost)
					{
						if(onCardLost != null)
						{
							ScenarioEvents.AbilityCardStateChangedEvent.Unsubscribe(abilityState, subscriber);

							await onCardLost(abilityCard);
						}
					}
				}
			);
		}
	}
}