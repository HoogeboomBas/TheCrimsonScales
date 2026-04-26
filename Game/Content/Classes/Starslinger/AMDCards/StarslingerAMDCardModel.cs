public abstract class StarslingerAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Starslinger/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}