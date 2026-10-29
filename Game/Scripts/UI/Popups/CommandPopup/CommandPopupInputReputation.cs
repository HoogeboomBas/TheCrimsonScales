using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputReputation : CommandPopupInput
{
	public override string Description => "Gain an amount of reputation. Can be negative.";

	public override string[] ArgumentDescriptions => ["reputation", "[amount]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 2)
		{
			if(int.TryParse(arguments[1], out int num))
			{
				GameController.Instance.SavedCampaign.AdjustReputation(num);
				commandPopup.SetOutputText(num < 0
					? $"lost {-num} reputation."
					: $"gained {num} reputation.");
			}
			else
			{
				commandPopup.SetOutputErrorText(arguments[1] + " cannot be converted to an integer reputation amount.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("reputation takes 2 parameters.");
		}
	}
}