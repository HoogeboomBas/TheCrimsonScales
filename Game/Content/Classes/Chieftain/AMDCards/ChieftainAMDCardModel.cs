public abstract class ChieftainAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Chieftain/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}