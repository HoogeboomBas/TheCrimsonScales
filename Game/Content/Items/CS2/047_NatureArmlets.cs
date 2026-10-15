public class NatureArmlets : CS2Item
{
	public override string Name => "Nature Armlets";
	public override int ItemNumber => 47;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 20;

	protected override void Subscribe()
	{
		base.Subscribe();

		//TODO: Implement Item
	}
}