public class CloakOfPockets : Prosperity2Item
{
	public override string Name => "Cloak of Pockets";
	public override int ItemNumber => 16;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 2;

	public override int SmallItemSlotCount => 2;
}