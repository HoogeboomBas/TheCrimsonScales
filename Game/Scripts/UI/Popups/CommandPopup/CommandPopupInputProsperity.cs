using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputProsperity : CommandPopupInput
{
	public override string Description => "Gain an amount of prosperity. Can be negative.";

	public override string[] ArgumentDescriptions => ["prosperity", "[amount]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 2)
		{
			if(int.TryParse(arguments[1], out int num))
			{
				GameController.Instance.SavedCampaign.AdjustProsperity(num);
				commandPopup.SetOutputText(num < 0
					? $"lost {-num} prosperity."
					: $"gained {num} prosperity.");
			}
			else
			{
				commandPopup.SetOutputErrorText(arguments[1] + " cannot be converted to an integer prosperity amount.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("prosperity takes 2 parameters.");
		}
	}
}