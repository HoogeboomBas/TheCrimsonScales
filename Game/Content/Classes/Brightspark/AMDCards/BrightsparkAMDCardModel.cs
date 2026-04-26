public abstract class BrightsparkAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Brightspark/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}