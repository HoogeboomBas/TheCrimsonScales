using System.Collections.Generic;

public class ChainguardAMDCard06 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 6;

	public override int? GetValue(AttackAbility.State state) => 1;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State state) =>
		state.Target.HasCondition(Chainguard.Shackle) ? [Conditions.Disarm] : null;
}