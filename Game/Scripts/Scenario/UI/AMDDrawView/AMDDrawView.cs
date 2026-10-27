using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class AMDDrawView : Control
{
	private const float OpenDistance = 400f;
	private const float DeckSizePerCard = 1f;

	[Export]
	private PackedScene _amdDrawCardscene;
	[Export]
	private Control _deckAnchor;

	[Export]
	private Control _discardContainer;
	[Export]
	private Control _discardAnchor;
	[Export]
	private TextureRect _discardTopCardTextureRect;
	[Export]
	public BetterButton FirstTerminalButton;

	[Export]
	private Control _secondTerminal;
	[Export]
	private Control _secondTerminalContainer;
	[Export]
	private Control _secondTerminalAnchor;
	[Export]
	private TextureRect _secondTerminalTextureRect;
	[Export]
	public BetterButton SecondTerminalButton;

	public override void _Ready()
	{
		base._Ready();

		Position = new Vector2(0, OpenDistance);

		FirstTerminalButton.SetEnabled(false, false);
		SecondTerminalButton.SetEnabled(false, false);

		Hide();
	}

	public async GDTask PeekCards(AbilityState potentialAbilityState, AMDCardDeck targetDeck, int cardsToPeek)
	{
		AMDCardDeck deck = targetDeck;

		Show();

		_secondTerminal.SetVisible(false);

		UpdateDrawPileSize(deck);

		await this.TweenPositionY(-280f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardableAsync();
		await GDTask.DelayFastForwardable(0.2f);

		int index = 0;
		int cardsPeeked = 0;

		while(cardsPeeked < cardsToPeek)
		{
			AMDCard newCard = deck.PeekCard(index);

			UpdateDrawPileSize(deck);

			AMDDrawCard drawCard = _amdDrawCardscene.Instantiate<AMDDrawCard>();
			AddChild(drawCard);
			await drawCard.DrawCard(newCard, _deckAnchor, _discardAnchor, 1f);

			_discardContainer.Visible = true;
			_discardTopCardTextureRect.Texture = newCard.GetTexture();

			ScenarioEvents.AMDCardPeeked.Parameters amdCardDrawnParameters =
				await ScenarioEvents.AMDCardPeekedEvent.CreatePrompt(
					new ScenarioEvents.AMDCardPeeked.Parameters(potentialAbilityState, newCard));

			if(amdCardDrawnParameters.PlaceAtDeckTop)
			{
				deck.MoveCardToTop(newCard);
				index++;
			}
			else if(amdCardDrawnParameters.PlaceAtDeckBottom)
			{
				deck.MoveCardToBottom(newCard);
			}
			else
			{
				index++;
			}

			cardsPeeked++;
		}

		// Move visuals away
		await this.TweenPositionY(OpenDistance, 0.3f).SetEasing(Easing.InBack).OnComplete(Hide).PlayFastForwardableAsync();
	}

	public async GDTask DrawCards(AttackAbility.State attackAbilityState)
	{
		AMDCardDeck deck = attackAbilityState.Performer.AMDCardDeck;

		Show();

		_discardContainer.Visible = deck.DiscardPile.Count > 0;
		_discardTopCardTextureRect.Texture = deck.DiscardPile.Count > 0 ? deck.DiscardPile[^1].GetTexture() : null;

		_secondTerminal.SetVisible(attackAbilityState.SingleTargetHasAdvantage != attackAbilityState.SingleTargetHasDisadvantage);
		_secondTerminalContainer.Visible = false;

		UpdateDrawPileSize(deck);
		UpdateDiscardPileSize(deck);

		await this.TweenPositionY(-120f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardableAsync();
		await GDTask.DelayFastForwardable(0.2f);

		AMDCardValue terminalCardValue = null;
		List<AMDCardValue> rollingCards = new List<AMDCardValue>();

		while(true)
		{
			AMDCard newCard = deck.DrawCard();

			UpdateDrawPileSize(deck);

			AMDDrawCard drawCard = _amdDrawCardscene.Instantiate<AMDDrawCard>();
			AddChild(drawCard);
			await drawCard.DrawCard(newCard, _deckAnchor, terminalCardValue == null ? _discardAnchor : _secondTerminalAnchor,
				terminalCardValue == null ? 1f : 1.5f);

			//TODO: If reshuffled, visualize that

			if(terminalCardValue == null)
			{
				UpdateDiscardPileSize(deck);
				_discardContainer.Visible = true;
				_discardTopCardTextureRect.Texture = newCard.GetTexture();
			}
			else
			{
				_secondTerminalContainer.Visible = true;
				_secondTerminalTextureRect.Texture = newCard.GetTexture();
			}

			// A drawn card can be overridden by an item or ability
			AMDCardValue newCardValue = await newCard.Draw(attackAbilityState);

			if(terminalCardValue == null)
			{
				if(newCard.Model.GetRolling(attackAbilityState))
				{
					if(!attackAbilityState.SingleTargetHasDisadvantage ||
					   attackAbilityState.SingleTargetHasAdvantage == attackAbilityState.SingleTargetHasDisadvantage)
					{
						rollingCards.Add(newCardValue);
						//await newCardValue.Apply(attackAbilityState);
					}
				}
				else
				{
					if(attackAbilityState.SingleTargetHasAdvantage != attackAbilityState.SingleTargetHasDisadvantage)
					{
						// Loop around to draw another card
						terminalCardValue = newCardValue;
						await GDTask.DelayFastForwardable(0.3f);
					}
					else
					{
						// Not rolling, no advantage or disadvantage, so done here
						terminalCardValue = newCardValue;
						await GDTask.DelayFastForwardable(0.5f);
						break;
					}
				}
			}
			else
			{
				// Had a previous terminal, so no more rolling allowed, and decide on which terminal to use
				int currentTerminalScore = terminalCardValue.GetAttackModifierValue(attackAbilityState);
				bool currentTerminalExtraEffect = terminalCardValue.GetHasExtraEffects(attackAbilityState);
				int newTerminalScore = newCardValue.GetAttackModifierValue(attackAbilityState);
				bool newTerminalExtraEffect = newCardValue.GetHasExtraEffects(attackAbilityState);

				//TODO: The code below does seem to work, but it can cause desyncs if the player adjusts their settings. Need to change this so the prompt contains the full check and checks the option there
				// if(attackAbilityState.SingleTargetHasAdvantage && attackAbilityState.Authority is Character &&
				//    AppController.Instance.DeviceOptions.DecideAMDWithAdvantage.Value)
				// {
				// 	// If the relevant option is enabled, always allow the player to decide between the AMD cards
				// 	await DecideAmbiguity();
				// }
				// else
				{
					if(currentTerminalScore > newTerminalScore)
					{
						if(currentTerminalExtraEffect && !newTerminalExtraEffect)
						{
							terminalCardValue = attackAbilityState.SingleTargetHasAdvantage ? terminalCardValue : newCardValue;
						}
						else if(!currentTerminalExtraEffect && newTerminalExtraEffect)
						{
							if(attackAbilityState.SingleTargetHasAdvantage && attackAbilityState.Authority is Character)
							{
								await DecideAmbiguity();
							}
						}
						else if(currentTerminalExtraEffect && newTerminalExtraEffect)
						{
							if(attackAbilityState.SingleTargetHasAdvantage && attackAbilityState.Authority is Character)
							{
								await DecideAmbiguity();
							}
						}
						else //if(!currentTerminalExtraEffect && !newTerminalExtraEffect)
						{
							terminalCardValue = attackAbilityState.SingleTargetHasAdvantage ? terminalCardValue : newCardValue;
						}
					}
					else if(currentTerminalScore < newTerminalScore)
					{
						if(currentTerminalExtraEffect && !newTerminalExtraEffect)
						{
							//Choice
							if(attackAbilityState.SingleTargetHasAdvantage && attackAbilityState.Authority is Character)
							{
								await DecideAmbiguity();
							}
						}
						else if(!currentTerminalExtraEffect && newTerminalExtraEffect)
						{
							terminalCardValue = attackAbilityState.SingleTargetHasAdvantage ? newCardValue : terminalCardValue;
						}
						else if(currentTerminalExtraEffect && newTerminalExtraEffect)
						{
							if(attackAbilityState.SingleTargetHasAdvantage && attackAbilityState.Authority is Character)
							{
								await DecideAmbiguity();
							}
						}
						else //if(!currentTerminalExtraEffect && !newTerminalExtraEffect)
						{
							terminalCardValue = attackAbilityState.SingleTargetHasAdvantage ? newCardValue : terminalCardValue;
						}
					}
					else //if(currentTerminalScore == newTerminalScore)
					{
						if(currentTerminalExtraEffect && !newTerminalExtraEffect)
						{
							terminalCardValue = attackAbilityState.SingleTargetHasAdvantage ? terminalCardValue : newCardValue;
						}
						else if(!currentTerminalExtraEffect && newTerminalExtraEffect)
						{
							terminalCardValue = attackAbilityState.SingleTargetHasAdvantage ? newCardValue : terminalCardValue;
						}
						else if(currentTerminalExtraEffect && newTerminalExtraEffect)
						{
							if(attackAbilityState.SingleTargetHasAdvantage && attackAbilityState.Authority is Character)
							{
								await DecideAmbiguity();
							}
						}
						else //if(!currentTerminalExtraEffect && !newTerminalExtraEffect)
						{
							// Exact same card
						}
					}
				}

				await GDTask.DelayFastForwardable(0.5f);
				break;
			}

			async GDTask DecideAmbiguity()
			{
				AMDCardSelectionPrompt.Answer answer =
					await PromptManager.Prompt(new AMDCardSelectionPrompt(null, () => "Select AMD card to use"), attackAbilityState.Authority);

				if(!answer.FirstTerminalSelected)
				{
					terminalCardValue = newCardValue;
				}
			}
		}

		// Move visuals away
		await this.TweenPositionY(OpenDistance, 0.3f).SetEasing(Easing.InBack).OnComplete(Hide).PlayFastForwardableAsync();

		foreach(AMDCardValue rollingCard in rollingCards)
		{
			await rollingCard.Apply(attackAbilityState);
		}

		await terminalCardValue.Apply(attackAbilityState);
	}

	private void UpdateDrawPileSize(AMDCardDeck deck)
	{
		_deckAnchor.Position = new Vector2(0f, -DeckSizePerCard * deck.DrawPile.Count);
	}

	private void UpdateDiscardPileSize(AMDCardDeck deck)
	{
		_discardAnchor.Position = new Vector2(0f, -DeckSizePerCard * deck.DiscardPile.Count);
	}
}