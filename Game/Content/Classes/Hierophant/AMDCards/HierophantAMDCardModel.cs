public abstract class HierophantAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Hierophant/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}