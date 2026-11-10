using System.Collections.Generic;
using Fractural.Tasks;

public partial class StarslingerTokenLostInTheStars : HexObject
{
	private Figure _character;
	public async GDTask Init(Figure character, Hex originHex)
	{
		await base.Init(originHex);
		_character = character;
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new CharacterInfoItem.Parameters((Character)_character));
	}
}
