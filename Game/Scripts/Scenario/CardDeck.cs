using System.Collections.Generic;

public class CardDeck<T>
	where T : IDeckCard
{
	public List<T> DrawPile { get; } = new List<T>();
	public List<T> DiscardPile { get; } = new List<T>();

	public bool MarkedForReshuffle;

	public CardDeck(IEnumerable<T> cards)
	{
		DrawPile.AddRange(cards);
		Reshuffle();
	}

	public T PeekCard()
	{
		if(DrawPile.Count == 0)
		{
			Reshuffle();
		}

		return DrawPile[^1];
	}

	public T DrawCard()
	{
		if(DrawPile.Count == 0)
		{
			Reshuffle();
		}

		T card = DrawPile[^1];
		DrawPile.RemoveAt(DrawPile.Count - 1);

		if(!card.RemoveAfterDraw)
		{
			DiscardPile.Add(card);
		}

		card.Drawn();

		MarkedForReshuffle |= card.Reshuffles;
		return card;
	}

	public void AddCard(T card, bool shuffleDrawPile)
	{
		DrawPile.Add(card);

		if(shuffleDrawPile)
		{
			ShuffleDrawPile();
		}
	}

	public void AddCardToBottom(T card)
	{
		DrawPile.Remove(card);
		DrawPile.Insert(0, card);
	}

	public void Reshuffle()
	{
		DrawPile.AddRange(DiscardPile);
		DiscardPile.Clear();

		ShuffleDrawPile();
	}

	public void ReshuffleIfMarked()
	{
		if(MarkedForReshuffle)
		{
			MarkedForReshuffle = false;
			Reshuffle();
		}
	}

	public void ShuffleDrawPile()
	{
		DrawPile.Shuffle(GameController.Instance.StateRNG);
	}
}