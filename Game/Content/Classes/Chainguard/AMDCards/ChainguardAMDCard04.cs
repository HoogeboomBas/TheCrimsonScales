public class ChainguardAMDCard04 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 4;

	public override bool GetRolling(AttackAbility.State state) => true;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override int? Swing => 3;
}