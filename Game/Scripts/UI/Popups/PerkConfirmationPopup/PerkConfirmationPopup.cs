using System.Collections.Generic;
using Godot;

public partial class PerkConfirmationPopup : Popup<PerkConfirmationPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCharacter SavedCharacter { get; init; }
		public int PerkIndex { get; init; }
	}

	[Export]
	private PackedScene _perkConfirmationPopupAMDCardScene;

	[Export]
	private Control _effectsContainer;
	[Export]
	private RichTextLabel _effectsLabel;

	[Export]
	private Control _cardsToRemoveContainer;
	[Export]
	private Control _cardsToRemoveParent;
	[Export]
	private Control _cardsToAddContainer;
	[Export]
	private Control _cardsToAddParent;

	[Export]
	private RichTextLabel _confirmationLabel;

	[Export]
	private BetterButton _cancelButton;
	[Export]
	private BetterButton _confirmButton;

	private readonly List<PerkConfirmationPopupAMDCard> _cards = new List<PerkConfirmationPopupAMDCard>();

	public override void _Ready()
	{
		base._Ready();

		_cancelButton.Pressed += OnCancelPressed;
		_confirmButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		PerkModel perkModel = PopupRequest.SavedCharacter.ClassModel.Perks[PopupRequest.PerkIndex];

		string effectsText = string.Empty;

		if(perkModel.IgnoreScenarioEffects)
		{
			effectsText += "Ignore scenario effects.";
		}

		RichTextParameters richTextParameters = _effectsLabel.GetRichTextParameters();
		if(perkModel.IgnoreItemMinusOneEffects)
		{
			if(!string.IsNullOrEmpty(effectsText))
			{
				effectsText += "\n";
			}

			effectsText += $"Ignore item {Icons.Inline(Icons.MinusOneCard, richTextParameters)} effects.";
		}

		string nonAMDDescription = perkModel.GetNonAMDDescription(richTextParameters);
		if(!string.IsNullOrEmpty(nonAMDDescription))
		{
			if(!string.IsNullOrEmpty(effectsText))
			{
				effectsText += "\n";
			}

			effectsText += nonAMDDescription;
		}

		_effectsContainer.SetVisible(!string.IsNullOrEmpty(effectsText));
		if(!string.IsNullOrEmpty(effectsText))
		{
			_effectsLabel.SetText(effectsText);
		}

		AddCards(perkModel.CardsToAdd, _cardsToAddContainer, _cardsToAddParent);
		AddCards(perkModel.CardsToRemove, _cardsToRemoveContainer, _cardsToRemoveParent);

		_confirmationLabel.Text = $"Acquire this perk for {PopupRequest.SavedCharacter.GetNameAndIcon()}?";
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(PerkConfirmationPopupAMDCard card in _cards)
		{
			card.QueueFree();
		}

		_cards.Clear();
	}

	private void AddCards(List<AMDCardModel> cardModels, Control container, Control parent)
	{
		container.SetVisible(cardModels.Count > 0);
		if(cardModels.Count == 0)
		{
			return;
		}

		foreach(AMDCardModel cardModel in cardModels)
		{
			PerkConfirmationPopupAMDCard card = _perkConfirmationPopupAMDCardScene.Instantiate<PerkConfirmationPopupAMDCard>();
			parent.AddChild(card);
			card.Init(cardModel, AMDCardOwner.Player1);
			_cards.Add(card);
		}
	}

	private void OnCancelPressed()
	{
		Close();
	}

	private void OnConfirmPressed()
	{
		PopupRequest.SavedCharacter.AcquirePerk(PopupRequest.PerkIndex);

		AppController.Instance.SaveGame();

		Close();
	}
}