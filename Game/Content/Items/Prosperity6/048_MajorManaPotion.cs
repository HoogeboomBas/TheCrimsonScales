public class MajorManaPotion : Prosperity5Item
{
	public override string Name => "Major Mana Potion";
	public override int ItemNumber => 48;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 10;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.InfuseWildElement(null, Owner);
					await AbilityCmd.InfuseWildElement(null, Owner);
				});
			}
		);
	}
}