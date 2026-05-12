public class MinorCurePotion : GHDesignsItem
{
	public override string Name => "Minor Cure Potion";
	public override int ItemNumber => 89;
	public override int ShopCount => 2;
	public override int Cost => 10;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool CanUseWhenStunned => true;

	protected override int AtlasIndex => 18;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.RemoveOneNegativeCondition(null, user);
				});
			}
		);
	}
}