using System.Collections.Generic;

public abstract class CommandPopupInput
{
	public abstract string Description { get; }
	public abstract string[] ArgumentDescriptions { get; }
	public int NumArguments => ArgumentDescriptions.Length;
	public abstract void HandleInput(List<string> arguments, CommandPopup popup);

	public string GetArgumentDescriptions()
	{
		string description = "";
		foreach(string arg in ArgumentDescriptions)
		{
			description += arg + " ";
		}

		return description;
	}
}