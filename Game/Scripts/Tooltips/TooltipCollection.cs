using System.Collections.Generic;

public class TooltipCollection
{
	public List<Tooltip> Tooltips { get; } = new List<Tooltip>();

	public void Add(Tooltip tooltip)
	{
		Tooltips.AddIfNew(tooltip);
	}
}