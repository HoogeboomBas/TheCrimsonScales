public abstract class SanctuaryRollingAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Art/AMDs/SanctuaryRolling.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 2;

	public override bool RemoveAfterDraw => true;

	public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
}