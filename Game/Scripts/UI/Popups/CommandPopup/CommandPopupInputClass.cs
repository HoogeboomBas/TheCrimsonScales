using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputClass : CommandPopupInput
{
	public override string Description => "Unlocks or removes a specific class.";

	public override string[] ArgumentDescriptions => ["class", "[unlock/remove]", "[class name]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 3)
		{
			bool unlock = true;
			if(arguments[1] == "remove")
			{
				unlock = false;
			}
			else if(arguments[1] != "unlock")
			{
				commandPopup.SetOutputErrorText("the second parameter of class must either be unlock or remove.");
				return;
			}

			ClassModel classModel = ModelDB.GetById<ClassModel>($"CLASS_MODEL.{arguments[2]}_MODEL");
			if(classModel == null)
			{
				commandPopup.SetOutputErrorText($"{arguments[2]} is not a valid class name.");
				return;
			}

			SavedCampaign savedCampaign = BetweenScenariosController.Instance.SavedCampaign;
			if(unlock)
			{
				if(savedCampaign.SavedClasses.ContainsKey(classModel.Id.ToString()))
				{
					commandPopup.SetOutputErrorText($"{classModel.Name} is already unlocked.");
					return;
				}

				savedCampaign.UnlockClass(classModel);
				commandPopup.SetOutputText($"unlocked {classModel.Name}.");
			}
			else
			{
				if(savedCampaign.Characters.Any(character => character.ClassModelId == classModel.Id.ToString()))
				{
					commandPopup.SetOutputErrorText($"Cannot remove {classModel.Name} as {classModel.Name} is currently in the party.");
					return;
				}

				if(!savedCampaign.SavedClasses.ContainsKey(classModel.Id.ToString()))
				{
					commandPopup.SetOutputErrorText($"{classModel.Name} is not currently unlocked.");
					return;
				}

				savedCampaign.SavedClasses.Remove(classModel.Id.ToString());
				commandPopup.SetOutputText($"removed {classModel.Name}.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("class takes 3 parameters.");
		}
	}
}