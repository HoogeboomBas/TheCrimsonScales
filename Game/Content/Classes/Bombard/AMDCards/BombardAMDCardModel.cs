public abstract class BombardAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Bombard/AMDCards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 3;
}