using System.Collections.Generic;

public class ChainguardAMDCard09 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 9;

	public override int? GetValue(AttackAbility.State state) => 2;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State state) => [Chainguard.Shackle];
}