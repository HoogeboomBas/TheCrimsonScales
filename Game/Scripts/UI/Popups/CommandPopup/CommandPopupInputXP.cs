using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputXP : CommandPopupInput
{
	public override string Description => "Adds an amount of xp to a specific character. Can be negative.";

	public override string[] ArgumentDescriptions => ["xp", "[class name], [amount]"];

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
				if(savedCharacter.Level == 1)
				{
					if(savedCharacter.XP + num < 0)
					{
						num = -savedCharacter.XP;
					}
				}
				else if(savedCharacter.XP + num < savedCharacter.ClassModel.XPLevelValues.Values[savedCharacter.Level - 2])
				{
					num = -(savedCharacter.XP - savedCharacter.ClassModel.XPLevelValues.Values[savedCharacter.Level - 2]);
				}

				savedCharacter.AddXP(num);
				commandPopup.SetOutputText(num < 0
					? $"{savedCharacter.ClassModel.Name} lost {-num} xp."
					: $"{savedCharacter.ClassModel.Name} gained {num} xp.");
			}
			else
			{
				commandPopup.SetOutputErrorText(arguments[2] + " cannot be converted to an integer xp amount.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("xp takes 3 parameters.");
		}
	}
}