public abstract class PartyAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Art/AMDs/Party.jpg";
	protected override int ColumnCount => 9;
	protected override int RowCount => 2;

	public override bool RemoveAfterDraw => true;

	public override bool GetRolling(AttackAbility.State attackAbilityState) => true;

	public override int? GetValue(AttackAbility.State attackAbilityState) => 0;
}