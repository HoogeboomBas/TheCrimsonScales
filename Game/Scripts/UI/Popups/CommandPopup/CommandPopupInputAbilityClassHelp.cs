using System.Collections.Generic;
using System;

public class CommandPopupInputAbilityClassHelp : CommandPopupInput
{
	public override string Description => "Provides all class names in screaming snake case for reference.";

	public override string[] ArgumentDescriptions => ["classhelp"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 1)
		{
			string text = "Classes: ";
			foreach(Type type in ModelDB.GetSubtypes<ClassModel>())
			{
				ClassModel model = ModelDB.GetByType<ClassModel>(type);
				string entry = model.Id.Entry;
				text += entry.Substring(0, entry.Length - 6) + ", ";
			}

			text = text.Substring(0, text.Length - 2);
			commandPopup.SetOutputText(text);
		}
		else
		{
			commandPopup.SetOutputErrorText("classhelp takes 1 parameter.");
		}
	}
}