using System.Collections.Generic;

public partial class DifficultTerrain : OverlayTile
{
	protected virtual string DisplayName => "Difficult Terrain";

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, DisplayName, "Any normal movement through this hex will cost two movement points.",
			xOffset: Hexes.Length > 1 ? -Map.HexWidth / 2 : 0));
	}
}