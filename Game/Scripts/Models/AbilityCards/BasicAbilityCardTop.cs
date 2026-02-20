using System.Collections.Generic;

public class BasicAbilityCardTop : AbilityCardSideModel
{
	public override AbilityCardSideType AbilityCardSideType => AbilityCardSideType.BasicTop;

	protected override List<AbilityCardAbility> GetAbilities() =>
	[
		new AbilityCardAbility(AttackAbility.Builder().WithDamage(2).Build())
	];
}