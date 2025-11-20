using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

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
					List<Hex> possibleHexes = new();
					possibleHexes.AddRange(
						RangeHelper.GetHexesInRange(character.Hex, 3)
							.Where(hex => hex.IsEmpty())
					);
					int minCount = 0;

					List<Hex> targetHexes = await AbilityCmd.SelectHexes(
						character,
						list => list.AddRange(possibleHexes),
						minSelectionCount: minCount,
						maxSelectionCount: 1,
						autoSelectIfMaxCountIsValidCount: false,
						hintText:  $"Select a hex to place the trap"
					);
					if(targetHexes.Count > 0)
					{
						foreach(Hex hex in targetHexes)
						{
							await AbilityCmd.CreateTrap(hex, "res://Content/OverlayTiles/Traps/BearTrap1H.tscn", damage: 0, conditions: [Conditions.Immobilize]);
						}
					}
				});
			}
		);
	}
}