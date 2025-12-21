public abstract class DefaultAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) =>
		owner == AMDCardOwner.Monsters ? "res://Art/AMDs/MonsterAMD.jpg" : $"res://Art/AMDs/Player{(int)owner}AMD.jpg";

	protected override int ColumnCount => 4;
	protected override int RowCount => 5;
}