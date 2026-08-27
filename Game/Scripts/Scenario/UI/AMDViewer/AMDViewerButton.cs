using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class AMDViewerButton : Control
{
	private class AMDCount(AMDCardModel amdModel, int deckCount = 0, int discardCount = 0)
	{
		public AMDCardModel Model = amdModel;
		public int DeckCount = deckCount;
		public int DiscardCount = discardCount;
	}

	[Export]
	private BetterButton _amdButton;

	[Export]
	private Control _amdView;

	[Export]
	private GridContainer _grid;

	[Export]
	private PackedScene _amdViewerBox;

	[Export]
	public Control ExtraDetailView;

	[Export]
	public RichTextLabel ExtraDetailLabel;

	private AMDCardDeck _amdCardDeck;

	private bool _lockedOpen;

	public override void _Ready()
	{
		base._Ready();

		_amdButton.MouseFilter = MouseFilterEnum.Stop;

		_amdButton.MouseEntered += OnMouseEntered;
		_amdButton.MouseExited += OnMouseExited;
		_amdButton.Pressed += OnButtonPressed;

		_amdView.Hide();
		ExtraDetailView.Hide();

		_amdView.TopLevel = true;
		ExtraDetailView.TopLevel = true;
	}

	public override void _Process(double delta)
	{
		MouseFilter = GameController.Instance.HexPin.Active
			? MouseFilterEnum.Stop
			: MouseFilterEnum.Ignore;
		_amdButton.MouseFilter = GameController.Instance.HexPin.Active
			? MouseFilterEnum.Stop
			: MouseFilterEnum.Ignore;
	}

	private void OnMouseEntered()
	{
		OpenViewer();
	}

	private void OnMouseExited()
	{
		if(!_lockedOpen)
		{
			_amdView.Hide();
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
			_amdView.Hide();
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

			bool clickedButton = _amdButton.GetGlobalRect().HasPoint(mousePosition);

			if(!clickedButton)
			{
				_lockedOpen = false;
				_amdView.Hide();
			}
		}
	}

	private void OpenViewer()
	{
		UpdateDeck();

		_amdView.Show();

		Vector2 popupPosition = GlobalPosition + new Vector2(-170, 0);

		_amdView.Position = new Vector2I((int)popupPosition.X, (int)popupPosition.Y);
	}

	public void UpdateDeck()
	{
		foreach(Node child in _grid.GetChildren())
		{
			child.QueueFree();
		}

		List<AMDCount> cardCounts = [];

		foreach(AMDCardModel amdModel in _amdCardDeck.DrawPile.Select(amd => amd.Model))
		{
			AMDCount modelCount = cardCounts.FirstOrDefault(cardCount =>
				cardCount.Model.ToString(new RichTextParameters()) == amdModel.ToString(new RichTextParameters()));
			if(modelCount == null)
			{
				cardCounts.Add(new AMDCount(amdModel, deckCount: 1));
			}
			else
			{
				modelCount.DeckCount++;
			}
		}

		foreach(AMDCardModel amdModel in _amdCardDeck.DiscardPile.Select(amd => amd.Model))
		{
			AMDCount modelCount = cardCounts.FirstOrDefault(cardCount =>
				cardCount.Model.ToString(new RichTextParameters()) == amdModel.ToString(new RichTextParameters()));
			if(modelCount == null)
			{
				cardCounts.Add(new AMDCount(amdModel, discardCount: 1));
			}
			else
			{
				modelCount.DiscardCount++;
			}
		}

		List<AMDCount> sortedCards = cardCounts
			.OrderBy(count => count.Model.Type switch
			{
				AMDCardType.Null => 0,
				AMDCardType.Value => 1,
				AMDCardType.Crit => 2,
				_ => 3
			})
			.ThenBy(count => count.Model.GetValue(null) ?? 0)
			.ThenBy(count => count.Model.GetRolling(null))
			.ThenBy(count => count.Model.ToString())
			.ToList();

		foreach(AMDCount cardCount in sortedCards)
		{
			AMDViewerBox box = _amdViewerBox.Instantiate<AMDViewerBox>();
			box.SetAMDViewerButton(this);
			box.SetAMD(cardCount.Model, cardCount.DeckCount, cardCount.DiscardCount);
			_grid.AddChild(box);
		}
	}

	public void SetAMDCardDeck(AMDCardDeck deck)
	{
		_amdCardDeck = deck;
	}
}