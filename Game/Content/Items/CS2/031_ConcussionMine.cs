using System.Collections.Generic;
using System.Linq;

public class ConcussionMine : CS2Item
{
	public override string Name => "Concussion Mine";
	public override int ItemNumber => 31;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 2;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.CreateTraps(damage: 0, range: 3, conditions: [Conditions.Stun], performer: user);
				});
			}
		);
	}
}