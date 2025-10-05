using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class CardSelectionView : Control
{
	[Export]
	private Control _container;
	[Export]
	private CardSelectionList _cardSelectionList;
	[Export]
	private Control _restButtons;
	[Export]
	private BetterButton _shortRestButton;
	[Export]
	private BetterButton _longRestButton;
	[Export]
	private Control _longRestIndicatorContainer;

	private bool _longRestSelected;
	private bool _restingEnabled;

	public IEnumerable<CardSelectionCard> Cards => _cardSelectionList.Cards;

	private event Action ShortRestPressedEvent;
	private event Action LongRestPressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_shortRestButton.Pressed += OnShortRestButtonPressed;
		_longRestButton.Pressed += OnLongRestButtonPressed;

		_shortRestButton.Scale = Vector2.Zero;
		_longRestButton.Scale = Vector2.Zero;
		_longRestIndicatorContainer.Scale = Vector2.Zero;

		Close();
	}

	public void Open(List<AbilityCard> cards,
		Action<CardSelectionCard> onCardPressed, Action<CardSelectionCard> onInitiativePressed,
		Action shortRestPressed, Action longRestPressed)
	{
		bool wasOpen = _cardSelectionList.Categories.Count > 0;
		TweenOut();

		ShortRestPressedEvent = shortRestPressed;
		LongRestPressedEvent = longRestPressed;

		List<CardSelectionListCategoryParameters> cardCategoryParameters = new List<CardSelectionListCategoryParameters>();

		cardCategoryParameters.Add(CreateCategoryParameters(cards, onCardPressed, onInitiativePressed,
			[CardState.Playing], CardSelectionListCategoryType.Playing));
		cardCategoryParameters.Add(CreateCategoryParameters(cards, onCardPressed, onInitiativePressed,
			[CardState.Persistent, CardState.PersistentLoss, CardState.Round, CardState.RoundLoss], CardSelectionListCategoryType.Active));
		cardCategoryParameters.Add(CreateCategoryParameters(cards, onCardPressed, onInitiativePressed,
			[CardState.Hand], CardSelectionListCategoryType.Hand));
		cardCategoryParameters.Add(CreateCategoryParameters(cards, onCardPressed, onInitiativePressed,
			[CardState.Discarded], CardSelectionListCategoryType.Discarded));
		cardCategoryParameters.Add(CreateCategoryParameters(cards, onCardPressed, onInitiativePressed,
			[CardState.Lost], CardSelectionListCategoryType.Lost));

		_cardSelectionList.Open(cardCategoryParameters, (cardA, cardB) => cardA.Model.Initiative.CompareTo(cardB.Model.Initiative));

		float delay = wasOpen ? 0.3f : 0f;
		foreach(CardSelectionListCategory category in _cardSelectionList.Categories)
		{
			if(category.HeaderContainer.Visible)
			{
				category.HeaderContainer.SetPosition(new Vector2(-600, category.HeaderContainer.Position.Y));
				GTweenSequenceBuilder.New()
					.AppendTime(delay)
					.Append(category.HeaderContainer.TweenPositionX(0f, 0.3f).SetEasing(Easing.OutBack))
					.Build().Play();

				delay += 0.03f;
			}

			foreach(CardSelectionCard card in category.Cards)
			{
				card.SetPosition(new Vector2(-600, card.Position.Y));
				GTweenSequenceBuilder.New()
					.AppendTime(delay)
					.Append(card.TweenPositionX(0f, 0.3f).SetEasing(Easing.OutBack))
					.Build().Play();

				delay += 0.03f;
			}
		}
	}

	public void Close()
	{
		TweenOut();

		_cardSelectionList.Close(false);

		_longRestIndicatorContainer.Scale = Vector2.Zero;
		_longRestButton.Position = new Vector2(0f, _longRestButton.Position.Y);

		SetRestingEnabled(false);
	}

	public void SetRestingEnabled(bool enabled)
	{
		if(_restingEnabled == enabled)
		{
			return;
		}

		_restingEnabled = enabled;

		if(_restingEnabled)
		{
			_shortRestButton.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).Play();
			_longRestButton.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).Play();
		}
		else
		{
			_shortRestButton.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).Play();
			_longRestButton.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).Play();
		}
	}

	public void SetLongRestSelected(bool selected)
	{
		if(_longRestSelected == selected)
		{
			return;
		}

		_longRestSelected = selected;

		if(_longRestSelected)
		{
			_longRestIndicatorContainer.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).Play();
		}
		else
		{
			_longRestIndicatorContainer.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).Play();
		}

		_longRestButton.TweenPositionX(_longRestSelected ? 30 : 0, 0.1f).SetEasing(Easing.OutBack).Play();
	}

	private void TweenOut()
	{
		float delay = 0f;
		foreach(CardSelectionListCategory category in _cardSelectionList.Categories)
		{
			if(category.HeaderContainer.Visible)
			{
				category.HeaderContainer.Reparent(_container);

				GTweenSequenceBuilder.New()
					.Append(category.HeaderContainer.TweenPositionX(category.HeaderContainer.Position.X, delay))
					.Append(category.HeaderContainer.TweenPositionX(-600f, 0.15f).SetEasing(Easing.InBack))
					.AppendCallback(category.HeaderContainer.QueueFree)
					.Build().Play();

				delay += 0.03f;
			}

			foreach(CardSelectionCard card in category.Cards)
			{
				card.Reparent(_container);

				GTweenSequenceBuilder.New()
					.Append(card.TweenPositionX(card.Position.X, delay))
					.Append(card.TweenPositionX(-600f, 0.15f).SetEasing(Easing.InBack))
					.AppendCallback(card.QueueFree)
					.Build().Play();

				delay += 0.03f;
			}
		}
	}

	private CardSelectionListCategoryParameters CreateCategoryParameters(List<AbilityCard> cards,
		Action<CardSelectionCard> onCardPressed, Action<CardSelectionCard> onInitiativePressed,
		CardState[] cardStates, CardSelectionListCategoryType categoryType)
	{
		return new CardSelectionListCategoryParameters(
			cards.Where(card => cardStates.Contains(card.CardState)).Select(card => card.SavedAbilityCard).ToList(),
			categoryType, onCardPressed, onInitiativePressed);
	}

	private void OnShortRestButtonPressed()
	{
		ShortRestPressedEvent?.Invoke();
	}

	private void OnLongRestButtonPressed()
	{
		LongRestPressedEvent?.Invoke();
	}
}