public abstract class ArtificerAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Artificer/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}