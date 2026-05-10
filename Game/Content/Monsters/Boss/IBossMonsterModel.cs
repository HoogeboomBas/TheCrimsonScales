using System.Collections.Generic;

public interface IBossMonsterModel
{
	string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters);
	string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters);
	IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster);
	IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster);
}