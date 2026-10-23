public abstract class IncarnateAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Incarnate/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 3;

	protected static bool InSpirit(Figure figure, IncarnateSpirit spirit)
	{
		return IncarnateCardSide.InSpirit(figure, spirit);
	}
}