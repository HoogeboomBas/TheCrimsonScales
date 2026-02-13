using System.Linq;
using Fractural.Tasks;

public class DrainingGreaves : CS1Item
{
	public override string Name => "Draining Greaves";
	public override int ItemNumber => 15;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 26;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeTurnEnded(
			canApply: character => character == Owner && character.TurnMovedHexes.Count >= 4,
			apply: async character =>
			{
				await Use(async user =>
				{
					Figure figure = await AbilityCmd.SelectFigure(character, list =>
					{
						foreach(Figure figure in RangeHelper.GetFiguresInRange(character.Hex, 1).Where(figure => figure.EnemiesWith(character)))
						{
							list.Add(figure);
						}
					});

					if(figure == null)
					{
						return;
					}

					await AbilityCmd.SufferDamage(figure, 1, character);
				});
			}
		);
	}
}