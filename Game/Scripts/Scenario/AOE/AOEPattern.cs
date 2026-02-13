using System.Collections.Generic;

public struct AOEPattern
{
	public List<AOEHex> LocalHexes { get; }

	public AOEPattern(List<AOEHex> localHexes)
	{
		LocalHexes = localHexes;
	}
}