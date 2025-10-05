using System.Collections.Generic;

public class CardManager
{
	private readonly Dictionary<SavedAbilityCard, AbilityCard> _cardDictionary = new Dictionary<SavedAbilityCard, AbilityCard>();

	public void Register(AbilityCard abilityCard)
	{
		_cardDictionary.Add(abilityCard.SavedAbilityCard, abilityCard);
	}

	public AbilityCard Get(SavedAbilityCard savedAbilityCard)
	{
		return _cardDictionary[savedAbilityCard];
	}
}