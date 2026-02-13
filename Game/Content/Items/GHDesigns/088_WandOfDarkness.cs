public class WandOfDarkness : GHDesignsItem
{
	public override string Name => "Wand of Darkness";
	public override int ItemNumber => 88;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 17;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.InfuseElement(null, Element.Dark, Owner);
				});
			}
		);
	}
}