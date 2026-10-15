public class DrakescaleArmor : GHRewardsItem
{
	public override string Name => "Drakescale Armor";
	public override int ItemNumber => 103;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 8;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeConditionsImmunity([Conditions.Wound1, Conditions.Poison1]);
	}
}