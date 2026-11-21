using System.Collections.Generic;

public class CommandPopupInputScenario : CommandPopupInput
{
	public override string Description => "Unlocks a specific scenario, using the scenario's number.";
	public override string[] ArgumentDescriptions => ["scenario", "[scenario number]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 2)
		{
			if(int.TryParse(arguments[1], out int num))
			{
				ScenarioModel scenario = ModelDB.GetById<ScenarioModel>($"SCENARIO_MODEL.SCENARIO{num:D3}");

				if(scenario == null)
				{
					commandPopup.SetOutputErrorText(arguments[1] + " is not a valid scenario number.");
				}
				else
				{
					BetweenScenariosController.Instance.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(scenario).Discover();
					commandPopup.SetOutputText($"unlocked scenario {arguments[1]}");
				}
			}
			else
			{
				commandPopup.SetOutputErrorText(arguments[1] + " cannot be converted to an integer scenario number.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("scenario takes 2 parameters.");
		}
	}
}