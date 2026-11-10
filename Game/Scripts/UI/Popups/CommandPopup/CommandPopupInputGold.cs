using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputGold : CommandPopupInput
{
	public override string Description => "Adds an amount of gold to a specific character. Can be negative.";

	public override string[] ArgumentDescriptions => ["gold", "[class name], [amount]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 3)
		{
			SavedCharacter savedCharacter =
				BetweenScenariosController.Instance.SavedCampaign.Characters.FirstOrDefault(character =>
					character.ClassModel.Id.Entry == $"{arguments[1]}_MODEL");
			if(savedCharacter == null)
			{
				commandPopup.SetOutputErrorText(arguments[1] + " is not a valid class or class is not currently in party.");
				return;
			}

			if(int.TryParse(arguments[2], out int num))
			{
				if(savedCharacter.Gold < -num)
				{
					num = -savedCharacter.Gold;
				}

				savedCharacter.AddGold(num);
				commandPopup.SetOutputText(num < 0
					? $"{savedCharacter.ClassModel.Name} lost {-num} gold."
					: $"{savedCharacter.ClassModel.Name} gained {num} gold.");
			}
			else
			{
				commandPopup.SetOutputErrorText(arguments[2] + " cannot be converted to an integer gold amount.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("gold takes 3 parameters.");
		}
	}
}