using System.Collections.Generic;

public sealed partial class CharacterStartHex : HexObject
{
	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, "Starting Hex", "Drag a character here to change its starting position.",
			sceneVerticalSize: 100f));
	}
}