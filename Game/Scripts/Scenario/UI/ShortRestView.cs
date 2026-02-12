using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GTweensGodot.Extensions;

public partial class ShortRestView : Control
{
	[Export]
	private CardView _cardView;
	[Export]
	private ChoiceButton _redrawButton;
	[Export]
	private ChoiceButton _confirmButton;

	private RandomNumberGenerator _rng;

	private Character _selectedCharacter;
	private AbilityCard _abilityCard;

	public event Action<AbilityCard> ConfirmedEvent;
	public event Action<AbilityCard> RedrawEvent;

	public override void _Ready()
	{
		base._Ready();

		_rng = new RandomNumberGenerator();

		_redrawButton.BetterButton.Pressed += OnRedrawPressed;
		_confirmButton.BetterButton.Pressed += OnConfirmPressed;

		Hide();
		this.TweenModulateAlpha(0f, 0f).Play(true);
	}

	public void Open(Character selectedCharacter, bool canRedraw)
	{
		// if(selectedCharacter.LostShortRestCard != null)
		// {
		// 	GD.PrintErr("Trying to open short rest view while a card has already been selected to be lost.");
		// 	return;
		// }

		_selectedCharacter = selectedCharacter;

		Show();
		this.TweenModulateAlpha(1f, 0.3f).Play();

		_redrawButton.SetActive(canRedraw);
		_confirmButton.SetActive(true);

		_rng.SetSeed((ulong)_selectedCharacter.ShortRestSeed);

		List<AbilityCard> discardedCards = _selectedCharacter.Cards.Where(card => card.CardState == CardState.Discarded).ToList();
		_abilityCard = discardedCards.PickRandom(_rng);

		_cardView.SetCard(_abilityCard.SavedAbilityCard);
	}

	public void Close()
	{
		_redrawButton.SetActive(false);
		_confirmButton.SetActive(false);

		this.TweenModulateAlpha(0f, 0.3f).OnComplete(Hide).Play();
	}

	private void OnRedrawPressed()
	{
		// _redrawButton.SetActive(false);
		//
		// List<AbilityCard> discardedCards = _selectedCharacter.Cards.Where(card => card.CardStatus == CardStatus.Discarded).ToList();
		// discardedCards.Remove(_abilityCard);
		// _abilityCard = discardedCards.PickRandom(_rng);
		//
		// _cardView.SetCard(_abilityCard);

		RedrawEvent?.Invoke(_abilityCard);
	}

	private void OnConfirmPressed()
	{
		ConfirmedEvent?.Invoke(_abilityCard);

		//Close();
	}
}