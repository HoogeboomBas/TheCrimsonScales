public abstract class MirefootAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Mirefoot/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 6;
}