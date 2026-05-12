public abstract class SpiritCallerAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/SpiritCaller/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}