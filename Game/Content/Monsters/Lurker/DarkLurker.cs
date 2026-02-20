using System.Collections.Generic;
using System.Linq;

public class DarkLurker : Lurker
{
	public override string Name => "Dark Lurker";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<Lurker>();
	public override IEnumerable<MonsterAbilityCardModel> Deck => HarrowerInfesterAbilityCard.Deck;
}