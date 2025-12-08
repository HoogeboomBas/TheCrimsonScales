public abstract class ChieftainAMDCardModel : AMDCardModel
{
	protected override string TexturePath => throw new System.NotImplementedException();
	protected override int ColumnCount => throw new System.NotImplementedException();
	protected override int RowCount => throw new System.NotImplementedException();

	public override AMDCardType Type => AMDCardType.Value;
}