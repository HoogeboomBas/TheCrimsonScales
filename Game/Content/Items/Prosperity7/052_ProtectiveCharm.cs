public class ProtectiveCharm : Prosperity7Item
{
	public override string Name => "Protective Charm";
	public override int ItemNumber => 52;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeConditionsImmunity([Conditions.Wound1, Conditions.Poison1]);
	}
}