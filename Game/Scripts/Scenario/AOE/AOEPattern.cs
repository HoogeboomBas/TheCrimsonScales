using System.Collections.Generic;

public class AOEPattern
{
	public List<AOEHex> LocalHexes { get; }

	public AOEPattern(List<AOEHex> localHexes)
	{
		LocalHexes = localHexes;
	}
}