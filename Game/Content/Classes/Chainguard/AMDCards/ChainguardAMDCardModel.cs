public abstract class ChainguardAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Chainguard/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}