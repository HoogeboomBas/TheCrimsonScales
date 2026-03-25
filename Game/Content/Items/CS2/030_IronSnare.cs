using System.Linq;

public class IronSnare : CS2Item
{
	public override string Name => "Iron Snare";
	public override int ItemNumber => 30;
	public override int ShopCount => 2;
	public override int Cost => 15;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 0;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.CreateTraps(damage: 0, range: 3, conditions: [Conditions.Immobilize], performer: user);
				});
			}
		);
	}
}