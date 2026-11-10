using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputHelp : CommandPopupInput
{
	public override string Description => "Provides the contents of the inputs of a specific command (e.g. help card).";
	public override string[] ArgumentDescriptions => ["help", "[command]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 2)
		{
			CommandPopupInput commandPopupInput =
				commandPopup.PossibleInputs.FirstOrDefault(popup => popup.ArgumentDescriptions.First() == arguments[1]);
			if(commandPopupInput == null)
			{
				commandPopup.SetOutputErrorText(arguments[1] + " is not a valid command.");
			}
			else
			{
				commandPopup.SetOutputText(commandPopupInput.GetArgumentDescriptions() + "\n" + commandPopupInput.Description);
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("help takes 2 parameters.");
		}
	}
}