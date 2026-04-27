public abstract class HollowpactAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Hollowpact/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}