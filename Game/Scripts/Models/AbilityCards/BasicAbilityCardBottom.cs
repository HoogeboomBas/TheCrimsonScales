using System.Collections.Generic;

public class BasicAbilityCardBottom : AbilityCardSideModel
{
	public override AbilityCardSideType AbilityCardSideType => AbilityCardSideType.BasicBottom;

	protected override List<AbilityCardAbility> GetAbilities() =>
	[
		new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build())
	];
}