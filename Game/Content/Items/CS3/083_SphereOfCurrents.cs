using System.Collections.Generic;
using System.Linq;
using Godot;

public class SphereOfCurrents : CS3Item
{
	public override string Name => "Sphere of Currents";
	public override int ItemNumber => 83;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 10;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					List<Hex> selectedHexes = await AbilityCmd.SelectHexes(user, list =>
						{
							list.AddRange(RangeHelper.GetHexesInRange(user.Hex, 3).Where(hex => hex.IsFeatureless()));
						},
						0, 2, false,
						$"Place water difficult terrain in up to two hexes within {Icons.HintText(Icons.Range)}3");

					foreach(Hex hex in selectedHexes)
					{
						await AbilityCmd.CreateDifficultTerrain(hex,
							ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/DifficultTerrain/Water1H.tscn"));
					}

					foreach(Figure figure in RangeHelper.GetFiguresInRange(user, 3, requiresLineOfSight: false))
					{
						if(figure.Hex.HasHexObjectOfType<Water>())
						{
							await AbilityCmd.AddCondition(null, figure, Conditions.Muddle);
						}
					}
				});
			}
		);
	}
}