using System.Collections.Generic;

public interface IBossMonsterModel
{
	string GetSpecial1Description(Monster monster);
	string GetSpecial2Description(Monster monster);
	IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster);
	IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster);
}