public abstract class RimehearthAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Rimehearth/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}