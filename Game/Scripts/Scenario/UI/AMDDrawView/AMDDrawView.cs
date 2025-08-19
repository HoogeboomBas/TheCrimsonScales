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
	private Control _discardAnchor;
	[Export]
	private Control _discardContainer;
	[Export]
	private TextureRect _discardTopCardTextureRect;

	public override void _Ready()
	{
		base._Ready();

		Position = new Vector2(0, OpenDistance);

		Hide();
	}

	public async GDTask<AMDCard> DrawCards(AttackAbility.State attackAbilityState)
	{
		AMDCardDeck deck = attackAbilityState.Performer.AMDCardDeck;

		Show();
		_discardContainer.Visible = deck.DiscardPile.Count > 0;
		_discardTopCardTextureRect.Texture = deck.DiscardPile.Count > 0 ? deck.DiscardPile[^1].GetTexture() : null;

		UpdateDrawPileSize(deck);
		UpdateDiscardPileSize(deck);

		await this.TweenPositionY(0f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardableAsync();
		await GDTask.DelayFastForwardable(0.2f);

		AMDCard terminal = null;
		//List<AMDCard> rollingCards = new List<AMDCard>();

		while(true)
		{
			AMDCard newCard = deck.DrawCard();

			UpdateDrawPileSize(deck);

			AMDDrawCard drawCard = _amdDrawCardscene.Instantiate<AMDDrawCard>();
			AddChild(drawCard);
			await drawCard.DrawCard(newCard, _deckAnchor, _discardAnchor);

			//TODO: If reshuffled, visualize that

			UpdateDiscardPileSize(deck);

			_discardContainer.Visible = true;
			_discardTopCardTextureRect.Texture = newCard.GetTexture();
			
			// A drawn card can be overridden by an item or ability
			newCard = await newCard.Draw(attackAbilityState);

			if(terminal == null)
			{
				if(newCard.Rolling)
				{
					//rollingCards.Add(newCard);

					if(!attackAbilityState.SingleTargetHasDisadvantage || attackAbilityState.SingleTargetHasAdvantage == attackAbilityState.SingleTargetHasDisadvantage)
					{
						await newCard.Apply(attackAbilityState);
					}
				}
				else
				{
					if(attackAbilityState.SingleTargetHasAdvantage != attackAbilityState.SingleTargetHasDisadvantage)
					{
						// Loop around to draw another card
						terminal = newCard;
						await GDTask.DelayFastForwardable(0.3f);
					}
					else
					{
						// Not rolling, no advantage or disadvantage, so done here
						terminal = newCard;
						await GDTask.DelayFastForwardable(0.5f);
						break;
					}
				}
			}
			else
			{
				// Had a previous terminal, so no more rolling allowed, and decide on which terminal to use
				(int currentTerminalScore, bool currentTerminalExtraEffect) = terminal.GetScore(attackAbilityState);
				(int newTerminalScore, bool newTerminalExtraEffect) = newCard.GetScore(attackAbilityState);

				if(currentTerminalScore > newTerminalScore)
				{
					if(currentTerminalExtraEffect && !newTerminalExtraEffect)
					{
						terminal = attackAbilityState.SingleTargetHasAdvantage ? terminal : newCard;
					}
					else if(!currentTerminalExtraEffect && newTerminalExtraEffect)
					{
						//Choice
						//terminal = terminal;
					}
					else if(currentTerminalExtraEffect && newTerminalExtraEffect)
					{
						//Choice
						//terminal = terminal;
					}
					else //if(!currentTerminalExtraEffect && !newTerminalExtraEffect)
					{
						terminal = attackAbilityState.SingleTargetHasAdvantage ? terminal : newCard;
					}
				}
				else if(currentTerminalScore < newTerminalScore)
				{
					if(currentTerminalExtraEffect && !newTerminalExtraEffect)
					{
						//Choice
						//terminal = terminal;
					}
					else if(!currentTerminalExtraEffect && newTerminalExtraEffect)
					{
						terminal = attackAbilityState.SingleTargetHasAdvantage ? newCard : terminal;
					}
					else if(currentTerminalExtraEffect && newTerminalExtraEffect)
					{
						//Choice
						//terminal = terminal;
					}
					else //if(!currentTerminalExtraEffect && !newTerminalExtraEffect)
					{
						terminal = attackAbilityState.SingleTargetHasAdvantage ? newCard : terminal;
					}
				}
				else //if(currentTerminalScore == newTerminalScore)
				{
					if(currentTerminalExtraEffect && !newTerminalExtraEffect)
					{
						terminal = attackAbilityState.SingleTargetHasAdvantage ? terminal : newCard;
					}
					else if(!currentTerminalExtraEffect && newTerminalExtraEffect)
					{
						terminal = attackAbilityState.SingleTargetHasAdvantage ? newCard : terminal;
					}
					else if(currentTerminalExtraEffect && newTerminalExtraEffect)
					{
						//Choice
						//terminal = terminal;
					}
					else //if(!currentTerminalExtraEffect && !newTerminalExtraEffect)
					{
						//terminal = terminal;
					}
				}

				await GDTask.DelayFastForwardable(0.5f);
				break;
			}
		}

		// if(!parameters.HasDisadvantage)
		// {
		// 	foreach(AMDCard rollingCard in rollingCards)
		// 	{
		// 		await rollingCard.Apply(parameters);
		// 	}
		// }

		await terminal.Apply(attackAbilityState);

		// Move visuals away
		await this.TweenPositionY(OpenDistance, 0.3f).SetEasing(Easing.InBack).OnComplete(Hide).PlayFastForwardableAsync();

		return terminal;
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