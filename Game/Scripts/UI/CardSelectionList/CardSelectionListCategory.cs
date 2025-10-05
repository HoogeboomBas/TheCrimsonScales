using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class CardSelectionListCategory : Control
{
	[Export]
	private PackedScene _cardSelectionCardScene;

	[Export]
	public Control HeaderContainer;
	[Export]
	private Label _headerLabel;
	[Export]
	private TextureRect _headerIcon;
	[Export]
	private Control _cardsContainer;

	private Comparison<SavedAbilityCard> _sortComparison;

	public CardSelectionListCategoryParameters Parameters { get; private set; }
	public List<CardSelectionCard> Cards { get; } = new List<CardSelectionCard>();

	public event Action<CardSelectionCard> CardMouseEnteredEvent;
	public event Action<CardSelectionCard> CardMouseExitedEvent;

	public void Init(CardSelectionListCategoryParameters parameters, Comparison<SavedAbilityCard> sortComparison)
	{
		Parameters = parameters;
		_sortComparison = sortComparison;

		HeaderContainer.SetVisible(Parameters.HasHeader);

		if(Parameters.HasHeader)
		{
			_headerLabel.SetText(Parameters.HeaderLabel);
			_headerIcon.SetTexture(ResourceLoader.Load<Texture2D>(Parameters.HeaderIconPath));
		}

		List<SavedAbilityCard> sortedCards = Parameters.Cards.ToList();
		sortedCards.Sort(_sortComparison);

		for(int i = 0; i < sortedCards.Count; i++)
		{
			SavedAbilityCard savedAbilityCard = sortedCards[i];
			CardSelectionCard card = CreateCard(savedAbilityCard, GetPosition(i));
			Cards.Add(card);
		}
	}

	public void Destroy(bool destroyCards)
	{
		if(destroyCards)
		{
			foreach(CardSelectionCard item in Cards)
			{
				item.QueueFree();
			}
		}

		QueueFree();
	}

	public void AddCard(SavedAbilityCard savedAbilityCard)
	{
		List<SavedAbilityCard> sortedCards = Cards.Select(cardSelectionCard => cardSelectionCard.SavedAbilityCard).ToList();
		sortedCards.Add(savedAbilityCard);
		sortedCards.Sort(_sortComparison);

		int index = sortedCards.IndexOf(savedAbilityCard);
		CardSelectionCard card = CreateCard(savedAbilityCard, GetPosition(index));
		Cards.Insert(index, card);

		card.SetPivotOffset(CardSelectionCard.Size * 0.5f);
		card.SetScale(Vector2.Zero);
		card.TweenScale(1f, 0.2f).SetEasing(Easing.OutBack).Play();
	}

	public void RemoveCard(SavedAbilityCard savedAbilityCard)
	{
		for(int i = 0; i < Cards.Count; i++)
		{
			CardSelectionCard cardSelectionCard = Cards[i];
			if(cardSelectionCard.SavedAbilityCard == savedAbilityCard)
			{
				cardSelectionCard.QueueFree();
				Cards.RemoveAt(i);
				break;
			}
		}
	}

	public void UpdateVisuals()
	{
		if(Cards.Count == 0)
		{
			HeaderContainer.SetVisible(false);
			SetCustomMinimumSize(Vector2.Zero);
			SetSize(CustomMinimumSize);
			return;
		}

		HeaderContainer.SetVisible(Parameters.HasHeader);

		for(int i = 0; i < Cards.Count; i++)
		{
			CardSelectionCard card = Cards[i];
			card.TweenPositionY(GetPosition(i), 0.3f).SetEasing(Easing.OutQuad).Play();
		}

		float headerSize = Parameters.HasHeader ? HeaderContainer.Size.Y : 0f;
		_cardsContainer.SetPosition(new Vector2(0f, headerSize));

		float totalSize = headerSize + Cards.Count * CardSelectionCard.Size.Y;
		SetCustomMinimumSize(new Vector2(CardSelectionCard.Size.X, totalSize));
		SetSize(CustomMinimumSize);
	}

	private CardSelectionCard CreateCard(SavedAbilityCard savedAbilityCard, float positionY)
	{
		CardSelectionCard card = _cardSelectionCardScene.Instantiate<CardSelectionCard>();
		_cardsContainer.AddChild(card);
		card.SetPosition(new Vector2(0f, positionY));
		card.Init(savedAbilityCard, true, Parameters.InitiativePressedEvent != null, Parameters.Type);

		card.CardPressedEvent += OnCardPressed;
		card.InitiativePressedEvent += OnInitiativePressed;
		card.MouseEnteredEvent += OnMouseEntered;
		card.MouseExitedEvent += OnMouseExited;

		return card;
	}

	private float GetPosition(int index)
	{
		return index * CardSelectionCard.Size.Y;
	}

	private void OnCardPressed(CardSelectionCard card)
	{
		Parameters.CardPressedEvent?.Invoke(card);
	}

	private void OnInitiativePressed(CardSelectionCard card)
	{
		Parameters.InitiativePressedEvent?.Invoke(card);
	}

	private void OnMouseEntered(CardSelectionCard card)
	{
		CardMouseEnteredEvent?.Invoke(card);
	}

	private void OnMouseExited(CardSelectionCard card)
	{
		CardMouseExitedEvent?.Invoke(card);
	}
}