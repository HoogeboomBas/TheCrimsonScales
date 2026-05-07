public abstract class FireKnightAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/FireKnight/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}