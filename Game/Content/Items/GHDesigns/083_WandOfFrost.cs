public class WandOfFrost : GHDesignsItem
{
	public override string Name => "Wand of Frost";
	public override int ItemNumber => 83;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 12;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.InfuseElement(Owner, [Element.Ice]);
				});
			}
		);
	}
}