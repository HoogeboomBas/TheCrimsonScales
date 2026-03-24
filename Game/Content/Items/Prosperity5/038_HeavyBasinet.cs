public class HeavyBasinet : Prosperity5Item
{
	public override string Name => "Heavy Basinet";
	public override int ItemNumber => 38;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override int MinusOneCount => 2;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeConditionsImmunity([Conditions.Stun, Conditions.Muddle]);
	}
}