public class BrilliantStone : CS4Item
{
	public override string Name => "Brilliant Stone";
	public override int ItemNumber => 98;
	public override int ShopCount => 1;
	public override int Cost => 25;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override int MaxUseCount => 2;

	protected override int AtlasIndex => 14;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.InfuseElement(null, Element.Light, Owner);
				});
			}
		);
	}
}