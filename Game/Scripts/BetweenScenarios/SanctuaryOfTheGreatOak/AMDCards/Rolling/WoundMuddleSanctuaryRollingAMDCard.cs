using System.Collections.Generic;

public class WoundMuddleSanctuaryRollingAMDCard : SanctuaryRollingAMDCardModel
{
	protected override int AtlasIndex => 4;

	public override int? GetValue(AttackAbility.State attackAbilityState) => 0;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) =>
	[
		Conditions.Wound1,
		Conditions.Muddle
	];
}