public abstract class GHRewardsItem : AtlasItemModel
{
	public override string ItemGroupId => "GH";

	protected override int ColumnCount => 8;
	protected override int RowCount => 6;
	protected override string TexturePath => "res://Content/Items/GHRewards/SpriteSheet.jpg";
}