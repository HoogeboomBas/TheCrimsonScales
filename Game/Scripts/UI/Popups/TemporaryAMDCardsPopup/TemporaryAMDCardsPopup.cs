using System.Collections.Generic;
using Godot;

public partial class TemporaryAMDCardsPopup : Popup<TemporaryAMDCardsPopup.Request>
{
	public class Request : PopupRequest
	{
		public string Title { get; init; }
		public AMDCardModel[] Cards { get; init; }
		public SavedCharacter Receiver { get; init; }
	}

	[Export]
	private PackedScene _cardScene;
	[Export]
	private Control _cardParent;

	[Export]
	private RichTextLabel _cardsAndReceiverLabel;

	[Export]
	private BetterButton _confirmButton;

	private readonly List<TemporaryAMDCardsPopupCard> _cards = new List<TemporaryAMDCardsPopupCard>();

	public override void _Ready()
	{
		base._Ready();

		_confirmButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		string title = PopupRequest.Title ?? "Temporary Attack Modifier Cards";
		_title.SetText(title);

		foreach(AMDCardModel amdCardModel in PopupRequest.Cards)
		{
			TemporaryAMDCardsPopupCard card = _cardScene.Instantiate<TemporaryAMDCardsPopupCard>();
			_cardParent.AddChild(card);
			card.Init(amdCardModel, AMDCardOwner.Player1);
			_cards.Add(card);
		}

		_cardsAndReceiverLabel.Text = $"{PopupRequest.Receiver.GetNameAndIcon()} received two temporary Attack Modifier cards.";
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(TemporaryAMDCardsPopupCard card in _cards)
		{
			card.QueueFree();
		}

		_cards.Clear();
	}

	private void OnConfirmPressed()
	{
		Close();
	}
}