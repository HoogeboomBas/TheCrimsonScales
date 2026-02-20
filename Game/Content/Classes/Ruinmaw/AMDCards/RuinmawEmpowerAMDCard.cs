public class RuinmawEmpowerAMDCard : EmpowerAMDCard
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Ruinmaw/AMDCards/AMDCards.png";
	protected override int ColumnCount => 3;
	protected override int RowCount => 2;
	protected override int AtlasIndex => 5;
	public override AMDCardType Type => AMDCardType.Value;
	public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
	public override int? Push => 1;
}