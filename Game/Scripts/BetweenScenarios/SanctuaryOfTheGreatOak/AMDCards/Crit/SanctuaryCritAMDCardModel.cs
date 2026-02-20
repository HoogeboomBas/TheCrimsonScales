public abstract class SanctuaryCritAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Art/AMDs/SanctuaryCrit.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 2;

	public override bool RemoveAfterDraw => true;

	public override AMDCardType Type => AMDCardType.Crit;
}