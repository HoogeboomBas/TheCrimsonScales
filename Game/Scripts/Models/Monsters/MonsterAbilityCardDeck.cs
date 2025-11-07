using System.Collections.Generic;

public class MonsterAbilityCardDeck : CardDeck<MonsterAbilityCard>
{
	public MonsterAbilityCardDeck(IEnumerable<MonsterAbilityCard> cards)
		: base(cards)
	{
	}
}