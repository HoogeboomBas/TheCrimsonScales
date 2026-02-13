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
					Hex targetHex = await AbilityCmd.SelectHex(
						user,
						list =>
							list.AddRange(RangeHelper.GetHexesInRange(user.Hex, 3)
								.Where(hex => hex.IsEmpty())),
						hintText: $"Select a hex to place the trap"
					);

					if(targetHex != null)
					{
						await AbilityCmd.CreateTrap(targetHex, "res://Content/OverlayTiles/Traps/BearTrap1H.tscn",
							conditions: [Conditions.Immobilize]);
					}
				});
			}
		);
	}
}