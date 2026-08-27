using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract partial class DeckViewerButton<T> : Control
	where T : IDeckCard
{
	public class CardCount(T card, int deckCount = 0, int discardCount = 0)
	{
		public T Card = card;
		public int DeckCount = deckCount;
		public int DiscardCount = discardCount;
	}

	[Export]
	private BetterButton _button;

	[Export]
	private Control _view;

	[Export]
	protected GridContainer Grid;

	[Export]
	protected PackedScene DeckViewerBox;

	[Export]
	public Control ExtraDetailView;

	private CardDeck<T> _cardDeck;

	private bool _lockedOpen;

	public override void _Ready()
	{
		base._Ready();

		_button.MouseEntered += OnMouseEntered;
		_button.MouseExited += OnMouseExited;
		_button.Pressed += OnButtonPressed;

		_view.Hide();
		ExtraDetailView.Hide();

		_view.TopLevel = true;
		ExtraDetailView.TopLevel = true;
	}

	private void OnMouseEntered()
	{
		OpenViewer();
	}

	private void OnMouseExited()
	{
		if(!_lockedOpen)
		{
			_view.Hide();
		}
	}

	private void OnButtonPressed()
	{
		_lockedOpen = !_lockedOpen;

		if(_lockedOpen)
		{
			OpenViewer();
		}
		else
		{
			_view.Hide();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(!_lockedOpen)
		{
			return;
		}

		if(@event is InputEventMouseButton mouseEvent &&
		   mouseEvent.ButtonIndex == MouseButton.Left &&
		   mouseEvent.Pressed)
		{
			Vector2 mousePosition = GetGlobalMousePosition();

			bool clickedButton = _button.GetGlobalRect().HasPoint(mousePosition);

			if(!clickedButton)
			{
				_lockedOpen = false;
				_view.Hide();
			}
		}
	}

	private void OpenViewer()
	{
		UpdateDeck();

		_view.Show();

		Vector2 popupPosition = GlobalPosition + new Vector2(-170, 0);

		_view.Position = new Vector2I((int)popupPosition.X, (int)popupPosition.Y);
	}

	public void UpdateDeck()
	{
		foreach(Node child in Grid.GetChildren())
		{
			child.QueueFree();
		}

		List<CardCount> cardCounts = [];

		foreach(T card in _cardDeck.DrawPile)
		{
			CardCount count = cardCounts.FirstOrDefault(cardCount => CardCountAvailable(cardCount, card));
			if(count == null)
			{
				cardCounts.Add(new CardCount(card, 1));
			}
			else
			{
				count.DeckCount++;
			}
		}

		foreach(T card in _cardDeck.DiscardPile)
		{
			CardCount count = cardCounts.FirstOrDefault(cardCount => CardCountAvailable(cardCount, card));
			if(count == null)
			{
				cardCounts.Add(new CardCount(card, discardCount: 1));
			}
			else
			{
				count.DiscardCount++;
			}
		}

		CreateCards(SortCardCounts(cardCounts));
	}

	public virtual void SetCardDeck(CardDeck<T> deck)
	{
		_cardDeck = deck;
	}

	public abstract bool CardCountAvailable(CardCount cardCount, T card);
	public abstract List<CardCount> SortCardCounts(List<CardCount> cardCounts);

	public abstract void CreateCards(List<CardCount> sortedCards);
}
