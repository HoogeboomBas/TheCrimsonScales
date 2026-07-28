using System.Data;
using System.Linq;

public class TranslocationDevice : CS1Item
{
	public override string Name => "Translocation Device";
	public override int ItemNumber => 27;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 46;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					Figure swapped = await AbilityCmd.SelectFigure(character, list =>
						list.AddRange(RangeHelper.GetFiguresInRange(character.Hex, 5, false)
							.Where(figure => character.EnemiesWith(figure) &&
							AbilityCmd.CanSwap(null, Owner, figure))),
						mandatory: false, hintText: () => "Choose an enemy to swap hexes with");
					if(swapped == null)
					{
						return;
					}

					await AbilityCmd.TrySwap(character, character, swapped);
				});
			}
		);
	}
}