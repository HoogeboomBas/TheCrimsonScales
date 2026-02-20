using System.Collections.Generic;

public partial class Obstacle : OverlayTile
{
	protected virtual string DisplayName => "Obstacle";

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, DisplayName,
			"Figures cannot normally move through this hex, except when flying or jumping.",
			xOffset: Hexes.Length > 1 ? -Map.HexWidth / 2 : 0,
			yOffset: Hexes.Length == 3 ? 95 : 0,
			sceneVerticalSize: Hexes.Length == 3 ? 310f : null));
	}
}