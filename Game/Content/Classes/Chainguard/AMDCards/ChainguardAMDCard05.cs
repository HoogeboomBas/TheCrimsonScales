using System.Collections.Generic;

public class ChainguardAMDCard05 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 12;

	public override int? GetValue(AttackAbility.State state) => 2;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State state) => [Conditions.Wound1];
}