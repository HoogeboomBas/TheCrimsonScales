using System.Collections.Generic;

public class MonsterAbilityCardDeck : CardDeck<MonsterAbilityCard>
{
	public MonsterAbilityCard ActiveCard { get; set; }

	public MonsterAbilityCardDeck(IEnumerable<MonsterAbilityCard> cards)
		: base(cards)
	{
	}
}