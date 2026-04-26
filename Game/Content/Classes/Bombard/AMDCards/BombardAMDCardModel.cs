public abstract class BombardAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Bombard/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}