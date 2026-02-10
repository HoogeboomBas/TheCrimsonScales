using System.Collections.Generic;

public abstract partial class ColonyToken : HexObject
{
	public const string AnyColony = "res://Content/Classes/AmberAegis/ColonyTokens/AnyColony.svg";

	protected abstract string DisplayName { get; }

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, DisplayName,
			$"If any figure enters this hex, this token is destroyed. When this token is destroyed, the Amber Aegis suffers {Icons.Inline(Icons.Damage)}1."));
	}
}

public interface IColonyToken
{
	static abstract string ScenePath { get; }
	static abstract string IconPath { get; }
	static abstract int MaxCount { get; }
}