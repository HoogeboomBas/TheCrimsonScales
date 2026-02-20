public class WonderStone : CS4Item
{
	public override string Name => "Wonder Stone";
	public override int ItemNumber => 100;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override int MaxUseCount => 2;

	protected override int AtlasIndex => 16;

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
			},
			canApplyMultipleTimesDuringAbility: true
		);
	}
}