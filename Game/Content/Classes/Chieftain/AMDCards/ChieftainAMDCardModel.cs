public abstract class ChieftainAMDCardModel : AMDCardModel
{
	protected override string TexturePath => "res://Art/AMDs/ChieftainAMD.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 5;

	public override AMDCardType Type => AMDCardType.Value;
}