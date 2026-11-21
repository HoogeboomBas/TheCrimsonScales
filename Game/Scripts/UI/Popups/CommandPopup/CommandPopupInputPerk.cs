using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputPerk : CommandPopupInput
{
	public override string Description => "Adds or removes a specific perk based off its index for a specific character. Perk indices start at 0.";

	public override string[] ArgumentDescriptions => ["perk", "[add/remove]", "[class name], [perk index]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 4)
		{
			bool add = true;
			if(arguments[1] == "remove")
			{
				add = false;
			}
			else if(arguments[1] != "add")
			{
				commandPopup.SetOutputErrorText("the second parameter of perk must either be add or remove.");
				return;
			}

			SavedCharacter savedCharacter =
				BetweenScenariosController.Instance.SavedCampaign.Characters.FirstOrDefault(character =>
					character.ClassModel.Id.Entry == $"{arguments[2]}_MODEL");
			if(savedCharacter == null)
			{
				commandPopup.SetOutputErrorText(arguments[2] + " is not a valid class or class is not currently in party.");
				return;
			}

			if(int.TryParse(arguments[3], out int num))
			{
				if(num < 0 || num >= savedCharacter.ClassModel.Perks.Count)
				{
					commandPopup.SetOutputErrorText(arguments[3] + $" is not a valid perk index for {savedCharacter.ClassModel.Name}.");
					return;
				}

				if(add)
				{
					if(savedCharacter.GetPerkAcquired(num))
					{
						commandPopup.SetOutputErrorText($"{savedCharacter.ClassModel.Name} has already acquired perk {num}.");
						return;
					}

					savedCharacter.AddAvailablePerk();
					savedCharacter.AcquirePerk(num);
					commandPopup.SetOutputText($"acquired perk {num} for {savedCharacter.ClassModel.Name}.");
				}
				else
				{
					if(savedCharacter.GetPerkAcquired(num))
					{
						savedCharacter.RemoveAvailablePerk();
						savedCharacter.RemovePerk(num);
						commandPopup.SetOutputText($"removed perk {num} for {savedCharacter.ClassModel.Name}.");
						return;
					}

					commandPopup.SetOutputErrorText($"{savedCharacter.ClassModel.Name} has not acquired perk {num}.");
				}
			}
			else
			{
				commandPopup.SetOutputErrorText(arguments[3] + " cannot be converted to an integer perk index.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("perk takes 4 parameters.");
		}
	}
}