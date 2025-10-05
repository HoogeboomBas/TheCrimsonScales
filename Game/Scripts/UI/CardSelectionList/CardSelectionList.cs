using System;
using System.Collections.Generic;
using Godot;

public partial class CardSelectionList : Control
{
	[Export]
	private PackedScene _cardSelectionListCategoryScene;

	[Export]
	private ScrollContainer _scrollContainer;
	[Export]
	private Control _categoriesContainer;
	[Export]
	private Control _container;

	public List<CardSelectionListCategory> Categories { get; } = new List<CardSelectionListCategory>();

	public IEnumerable<CardSelectionCard> Cards
	{
		get
		{
			foreach(CardSelectionListCategory category in Categories)
			{
				foreach(CardSelectionCard cardSelectionCard in category.Cards)
				{
					yield return cardSelectionCard;
				}
			}
		}
	}

	public override void _Ready()
	{
		base._Ready();

		Close();
	}

	public void Open(List<SavedAbilityCard> cards,
		Action<CardSelectionCard> cardPressed, Action<CardSelectionCard> initiativePressed,
		Comparison<SavedAbilityCard> sortComparison)
	{
		CardSelectionListCategoryParameters parameters =
			new CardSelectionListCategoryParameters(cards, CardSelectionListCategoryType.None, cardPressed, initiativePressed);
		Open([parameters], sortComparison);
	}

	public void Open(List<CardSelectionListCategoryParameters> cardCategoryParameters,
		Comparison<SavedAbilityCard> sortComparison)
	{
		foreach(CardSelectionListCategory category in Categories)
		{
			category.QueueFree();
		}

		Categories.Clear();

		foreach(CardSelectionListCategoryParameters cardSelectionListCategoryParameters in cardCategoryParameters)
		{
			CardSelectionListCategory category = _cardSelectionListCategoryScene.Instantiate<CardSelectionListCategory>();
			_categoriesContainer.AddChild(category);
			category.Init(cardSelectionListCategoryParameters, sortComparison);
			category.CardMouseEnteredEvent += OnMouseEntered;
			category.CardMouseExitedEvent += OnMouseExited;

			Categories.Add(category);
		}

		UpdateScrollRect();
	}

	public void Close(bool destroyCards = true)
	{
		foreach(CardSelectionListCategory category in Categories)
		{
			category.Destroy(destroyCards);
		}

		Categories.Clear();

		_categoriesContainer.CustomMinimumSize = new Vector2(_categoriesContainer.CustomMinimumSize.X, 0f);
		_categoriesContainer.Size = _categoriesContainer.CustomMinimumSize;
		_scrollContainer.MouseFilter = MouseFilterEnum.Ignore;
	}

	public void AddCard(SavedAbilityCard savedAbilityCard, CardSelectionListCategoryType type = CardSelectionListCategoryType.None)
	{
		bool added = false;
		foreach(CardSelectionListCategory category in Categories)
		{
			if(category.Parameters.Type == type)
			{
				category.AddCard(savedAbilityCard);
				added = true;
				break;
			}
		}

		if(!added)
		{
			Log.Error($"Could not add card {savedAbilityCard.Model.Name} with CardSelectionListCategoryType {type} to any category.");
		}

		UpdateScrollRect();
	}

	public void RemoveCard(SavedAbilityCard savedAbilityCard)
	{
		bool removed = false;
		foreach(CardSelectionListCategory category in Categories)
		{
			foreach(CardSelectionCard cardSelectionCard in category.Cards)
			{
				if(cardSelectionCard.SavedAbilityCard == savedAbilityCard)
				{
					category.RemoveCard(savedAbilityCard);
					removed = true;
					break;
				}
			}
		}

		if(!removed)
		{
			Log.Error($"Could not remove card {savedAbilityCard.Model.Name} to any category.");
		}

		UpdateScrollRect();
	}

	private void UpdateScrollRect()
	{
		float totalSize = 0f;
		foreach(CardSelectionListCategory category in Categories)
		{
			category.SetPosition(new Vector2(0f, totalSize));
			category.UpdateVisuals();

			totalSize += category.Size.Y;
		}

		_scrollContainer.SetSize(new Vector2(CardSelectionCard.Size.X, _scrollContainer.Size.Y));
		_scrollContainer.SetPosition(new Vector2(0f, _scrollContainer.Position.Y));
		_categoriesContainer.SetCustomMinimumSize(new Vector2(_categoriesContainer.Size.X, totalSize));
		_categoriesContainer.SetSize(_categoriesContainer.CustomMinimumSize);
		_scrollContainer.SetMouseFilter(_categoriesContainer.Size.Y > _scrollContainer.Size.Y ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore);
		_categoriesContainer.SetMouseFilter(_categoriesContainer.Size.Y > _scrollContainer.Size.Y ? MouseFilterEnum.Pass : MouseFilterEnum.Stop);
	}

	private void OnMouseEntered(CardSelectionCard card)
	{
		AppController.Instance.CardSelectionCardPreview.Focus(card);
	}

	private void OnMouseExited(CardSelectionCard card)
	{
		AppController.Instance.CardSelectionCardPreview.Unfocus(card);
	}
}