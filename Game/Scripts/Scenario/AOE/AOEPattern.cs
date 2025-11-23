using System.Collections.Generic;

public struct AOEPattern
{
	public List<AOEHex> Hexes { get; }

	public AOEPattern(List<AOEHex> hexes)
	{
		Hexes = hexes;
	}
}