public abstract class DefaultOtherAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Art/AMDs/Other.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 2;
}