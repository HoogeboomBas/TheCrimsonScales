using Fractural.Tasks;

public abstract class ShardrenderAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Shardrender/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;

	protected async GDTask<bool> MoveCharacterTokenBack(Character character, int count, bool canBeDifferent = true)
	{
		return await ShardrenderCardSide.MoveCharacterTokenBack(character, count, canBeDifferent);
	}
}