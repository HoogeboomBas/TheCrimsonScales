public abstract class LuminaryAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Luminary/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}