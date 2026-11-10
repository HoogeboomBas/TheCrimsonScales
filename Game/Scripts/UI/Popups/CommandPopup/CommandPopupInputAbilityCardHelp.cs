using System.Collections.Generic;

public class CommandPopupInputAbilityCardHelp : CommandPopupInput
{
	public override string Description => "Provides all cards for a certain class in screaming snake case for reference.";

	public override string[] ArgumentDescriptions => ["cardhelp", "[class name]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 2)
		{
			ClassModel classModel = ModelDB.GetById<ClassModel>($"CLASS_MODEL.{arguments[1]}_MODEL");

			if(classModel == null)
			{
				commandPopup.SetOutputErrorText(arguments[1] + " is not a valid class name.");
				return;
			}

			string text = $"{classModel.Name} ability cards: ";
			foreach(AbilityCardModel cardModel in classModel.AbilityCards)
			{
				text += cardModel.Id.Entry + ", ";
			}

			text = text.Substring(0, text.Length - 2);
			commandPopup.SetOutputText(text);
		}
		else
		{
			commandPopup.SetOutputErrorText("cardhelp takes 2 parameters.");
		}
	}
}