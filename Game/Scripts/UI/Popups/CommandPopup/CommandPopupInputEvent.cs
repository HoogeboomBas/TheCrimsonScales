using System.Collections.Generic;

public class CommandPopupInputEvent : CommandPopupInput
{
	public override string Description => "Adds or removes a specific event from its deck by event number.";
	public override string[] ArgumentDescriptions => ["event", "[add/remove]", "[city/road]", "[event number]"];

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
				commandPopup.SetOutputErrorText("the second parameter of event must either be add or remove.");
				return;
			}

			bool city = true;
			if(arguments[2] == "road")
			{
				city = false;
			}
			else if(arguments[2] != "city")
			{
				commandPopup.SetOutputErrorText("the third parameter of event must either be city or road.");
				return;
			}

			SavedCampaign savedCampaign = BetweenScenariosController.Instance.SavedCampaign;

			if(int.TryParse(arguments[3], out int num))
			{
				List<string> eventDeckIds = city
					? savedCampaign.SavedEvents.CityEventDeckIds
					: savedCampaign.SavedEvents.RoadEventDeckIds;

				EventModel eventModel = ModelDB.GetByEntry<EventModel>($"{(city ? "CITY" : "ROAD")}{num:D2}");
				if(eventModel == null)
				{
					commandPopup.SetOutputErrorText($"{num} is not a valid {(city ? "city" : "road")} event number.");
					return;
				}
				if(add)
				{
					if(eventDeckIds.Contains(eventModel.Id.ToString()))
					{
						commandPopup.SetOutputErrorText($"{(city ? "city" : "road")} event {num} is already in the {(city ? "city" : "road")} event deck.");
						return;
					}

					if(city)
					{
						savedCampaign.SavedEvents.AddCityEventToDeck(eventModel, BetweenScenariosController.Instance.RNG);
					}
					else
					{
						savedCampaign.SavedEvents.AddRoadEventToDeck(eventModel, BetweenScenariosController.Instance.RNG);
					}
					commandPopup.SetOutputText($"added {(city ? "city" : "road")} event {num} to the {(city ? "city" : "road")} event deck.");
				}
				else
				{
					if(!eventDeckIds.Contains(eventModel.Id.ToString()))
					{
						commandPopup.SetOutputErrorText($"{(city ? "city" : "road")} event {num} is not in the {(city ? "city" : "road")} event deck.");
						return;
					}

					if(city)
					{
						savedCampaign.SavedEvents.CityEventDeckIds.Remove(eventModel.Id.ToString());
						savedCampaign.SavedEvents.TopCityDeckQueueIds.Remove(eventModel.Id.ToString());
					}
					else
					{
						savedCampaign.SavedEvents.RoadEventDeckIds.Remove(eventModel.Id.ToString());
						savedCampaign.SavedEvents.TopRoadDeckQueueIds.Remove(eventModel.Id.ToString());
					}
					commandPopup.SetOutputText($"removed {(city ? "city" : "road")} event {num} from the {(city ? "city" : "road")} event deck.");
				}
			}
			else
			{
				commandPopup.SetOutputErrorText(arguments[1] + " cannot be converted to an integer event number.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("event takes 4 parameters.");
		}
	}
}