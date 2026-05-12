public abstract class HollowpactAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Hollowpact/AMDCards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 3;
}