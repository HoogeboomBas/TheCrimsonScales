public abstract class ThornreaperAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Thornreaper/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}