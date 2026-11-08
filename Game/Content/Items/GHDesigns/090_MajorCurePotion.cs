public class MajorCurePotion : GHDesignsItem, IPotion
{
	public override string Name => "Major Cure Potion";
	public override int ItemNumber => 90;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool CanUseWhileStunned => true;

	protected override int AtlasIndex => 19;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.RemoveAllNegativeConditions(user);
				});
			}
		);
	}
}