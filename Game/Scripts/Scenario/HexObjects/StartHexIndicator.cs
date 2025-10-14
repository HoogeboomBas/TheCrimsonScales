using System.Collections.Generic;

public sealed partial class StartHexIndicator : HexObject
{
	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, "Starting Hex", "A starting hex.", sceneVerticalSize: 100f));
	}
}