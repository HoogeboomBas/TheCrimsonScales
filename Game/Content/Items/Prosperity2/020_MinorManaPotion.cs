public class MinorManaPotion : Prosperity2Item
{
	public override string Name => "Minor Mana Potion";
	public override int ItemNumber => 20;
	public override int ShopCount => 2;
	public override int Cost => 10;
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
				});
			}
		);
	}
}