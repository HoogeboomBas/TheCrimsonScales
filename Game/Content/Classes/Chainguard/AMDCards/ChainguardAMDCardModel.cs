public abstract class ChainguardAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Art/AMDs/ChainguardAMD.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;

	public override AMDCardType Type => AMDCardType.Value;
}