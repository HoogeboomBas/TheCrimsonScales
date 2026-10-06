public abstract class BruiserAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Bruiser/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}