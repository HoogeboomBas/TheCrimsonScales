using System.Collections.Generic;

public class MonsterAbilityCardDeck : CardDeck<MonsterAbilityCard>
{
	public MonsterAbilityCard ActiveMonsterAbilityCard { get; set; }
	public MonsterAbilityCardDeck(IEnumerable<MonsterAbilityCard> cards)
		: base(cards)
	{
	
	}
}