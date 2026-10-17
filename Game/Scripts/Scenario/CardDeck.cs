using System.Collections.Generic;
using System.Linq;

public class CardDeck<T>
	where T : IDeckCard
{
	public List<T> DrawPile { get; } = new List<T>();
	public List<T> DiscardPile { get; } = new List<T>();

	public CardDeck(IEnumerable<T> cards)
	{
		DrawPile.AddRange(cards);
		Reshuffle();
	}

	public T PeekCard(int position)
	{
		if(DrawPile.Count < position + 1)
		{
			Reshuffle();
		}

		return DrawPile[DrawPile.Count - 1 - position];
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

	public void MoveCardToTop(T card)
	{
		DrawPile.Remove(card);
		DrawPile.Add(card);
	}

	public void MoveCardToBottom(T card)
	{
		DrawPile.Remove(card);
		DrawPile.Insert(0, card);
	}

	public void RemoveCard(T card)
	{
		DrawPile.Remove(card);
	}

	public void Reshuffle()
	{
		DrawPile.AddRange(DiscardPile);
		DiscardPile.Clear();

		ShuffleDrawPile();
	}

	public void ReshuffleIfMarked()
	{
		if(DiscardPile.Any(card => card.Reshuffles))
		{
			Reshuffle();
		}
	}

	public void ShuffleDrawPile()
	{
		DrawPile.Shuffle(GameController.Instance.StateRNG);
	}
}