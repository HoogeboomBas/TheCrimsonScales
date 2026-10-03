using Godot;

public abstract partial class DeckViewerBox<TCard, TViewerButton> : Control
	where TCard : IDeckCard
	where TViewerButton : DeckViewerButton<TCard>
{
	[Export]
	private PackedScene _deckCount;

	[Export]
	private PackedScene _discardCount;

	[Export]
	private Container _countContainer;

	protected TViewerButton DeckViewerButton;

	protected TCard Card;

	public virtual void SetCard(TCard card, int deckCount, int discardCount)
	{
		Card = card;

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		for(int i = 0; i < deckCount; i++)
		{
			Control circle = _deckCount.Instantiate<Control>();
			_countContainer.AddChild(circle);
		}

		for(int i = 0; i < discardCount; i++)
		{
			Control circle = _discardCount.Instantiate<Control>();
			_countContainer.AddChild(circle);
		}
	}

	protected virtual void OnMouseEntered()
	{
		DeckViewerButton.ExtraDetailView.Show();

		Vector2 popupPosition = DeckViewerButton.GlobalPosition + new Vector2(-520, -65);

		DeckViewerButton.ExtraDetailView.Position = new Vector2I((int)popupPosition.X, (int)popupPosition.Y);
	}

	private void OnMouseExited()
	{
		DeckViewerButton.ExtraDetailView.Hide();
	}

	public void SetDeckViewerButton(TViewerButton amdViewerButton)
	{
		DeckViewerButton = amdViewerButton;
	}
}