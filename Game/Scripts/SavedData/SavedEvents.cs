using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedEvents
{
	private static EventModel[] StartingCityEventDeck =
	[
		ModelDB.Event<City01>(),
		ModelDB.Event<City02>(),
		ModelDB.Event<City03>(),
		ModelDB.Event<City04>(),
		ModelDB.Event<City05>(),
		// ModelDB.Event<City06>(),
		ModelDB.Event<City07>(),
		ModelDB.Event<City08>(),
		// ModelDB.Event<City09>(),
		ModelDB.Event<City10>(),
		ModelDB.Event<City11>(),
		ModelDB.Event<City12>(),
		ModelDB.Event<City13>(),
		ModelDB.Event<City14>(),
		ModelDB.Event<City15>(),
		ModelDB.Event<City16>(),
		// ModelDB.Event<City17>()
		ModelDB.Event<City18>(),
		ModelDB.Event<City19>(),
		ModelDB.Event<City20>(),
		ModelDB.Event<City21>(),
		ModelDB.Event<City22>(),
		ModelDB.Event<City23>(),
		ModelDB.Event<City24>(),
		ModelDB.Event<City25>(),
		ModelDB.Event<City26>(),
		ModelDB.Event<City27>(),
		ModelDB.Event<City28>(),
		ModelDB.Event<City29>(),
		ModelDB.Event<City30>(),
	];

	private static EventModel[] StartingRoadEventDeck =
	[
		// ModelDB.Event<Road01>(),
		ModelDB.Event<Road02>(),
		ModelDB.Event<Road03>(),
		ModelDB.Event<Road04>(),
		ModelDB.Event<Road05>(),
		ModelDB.Event<Road06>(),
		ModelDB.Event<Road07>(),
		ModelDB.Event<Road08>(),
		ModelDB.Event<Road09>(),
		// ModelDB.Event<Road10>(),
		ModelDB.Event<Road11>(),
		// ModelDB.Event<Road12>(),
		ModelDB.Event<Road13>(),
		ModelDB.Event<Road14>(),
		ModelDB.Event<Road15>(),
		ModelDB.Event<Road16>(),
		ModelDB.Event<Road17>(),
		ModelDB.Event<Road18>(),
		ModelDB.Event<Road19>(),
		ModelDB.Event<Road20>(),
		ModelDB.Event<Road21>(),
		ModelDB.Event<Road22>(),
		ModelDB.Event<Road23>(),
		ModelDB.Event<Road24>(),
		// // ModelDB.Event<Road25>(),
		ModelDB.Event<Road26>(),
		ModelDB.Event<Road27>(),
		ModelDB.Event<Road28>(),
		ModelDB.Event<Road29>(),
		ModelDB.Event<Road30>(),
	];

	[JsonProperty]
	public List<string> CityEventDeckIds { get; private set; }

	[JsonProperty]
	public List<string> RoadEventDeckIds { get; private set; }

	[JsonProperty]
	public List<SavedEventState> SavedEventStates { get; private set; } = new List<SavedEventState>();

	[JsonProperty]
	public bool CanDrawCityEvent { get; private set; }

	public SavedEvents()
	{
		CityEventDeckIds = StartingCityEventDeck.Select(eventModel => eventModel.Id.ToString()).ToList();
		RoadEventDeckIds = StartingRoadEventDeck.Select(eventModel => eventModel.Id.ToString()).ToList();

		RandomNumberGenerator tempRNG = new RandomNumberGenerator();
		tempRNG.Randomize();
		CityEventDeckIds.Shuffle(tempRNG);
		RoadEventDeckIds.Shuffle(tempRNG);
	}

	public void AddSavedEventState(SavedEventState savedEventState)
	{
		SavedEventStates.Add(savedEventState);
	}

	public void RemoveSavedEventState(SavedEventState savedEventState)
	{
		SavedEventStates.Remove(savedEventState);
	}

	public void OnScenarioEnded()
	{
		CanDrawCityEvent = true;
		SavedEventStates.Clear();
	}

	public EventModel DrawCityEvent()
	{
		if(CityEventDeckIds.Count == 0)
		{
			throw new Exception("The City Event deck is empty!");
		}

		CanDrawCityEvent = false;

		EventModel eventModel = ModelDB.GetById<EventModel>(CityEventDeckIds[CityEventDeckIds.Count - 1]);
		CityEventDeckIds.RemoveAt(CityEventDeckIds.Count - 1);
		return eventModel;
	}

	public EventModel DrawRoadEvent()
	{
		if(RoadEventDeckIds.Count == 0)
		{
			throw new Exception("The Road Event deck is empty!");
		}

		EventModel eventModel = ModelDB.GetById<EventModel>(RoadEventDeckIds[RoadEventDeckIds.Count - 1]);
		RoadEventDeckIds.RemoveAt(RoadEventDeckIds.Count - 1);
		return eventModel;
	}

	public void ReturnCityEventToBottom(EventModel eventModel)
	{
		if(eventModel.EventType != EventType.City)
		{
			Log.Error("Trying to return an event of the wrong type!");
			return;
		}

		CityEventDeckIds.Insert(0, eventModel.Id.ToString());
	}

	public void ReturnRoadEventToBottom(EventModel eventModel)
	{
		if(eventModel.EventType != EventType.Road)
		{
			Log.Error("Trying to return an event of the wrong type!");
			return;
		}

		RoadEventDeckIds.Insert(0, eventModel.Id.ToString());
	}

	public void AddCityEventToDeck(EventModel eventModel, RandomNumberGenerator rng)
	{
		if(eventModel.EventType != EventType.City)
		{
			Log.Error("Trying to add an event of the wrong type!");
			return;
		}

		CityEventDeckIds.Add(eventModel.Id.ToString());
		CityEventDeckIds.Shuffle(rng);
	}

	public void AddRoadEventToDeck(EventModel eventModel, RandomNumberGenerator rng)
	{
		if(eventModel.EventType != EventType.Road)
		{
			Log.Error("Trying to add an event of the wrong type!");
			return;
		}

		RoadEventDeckIds.Add(eventModel.Id.ToString());
		RoadEventDeckIds.Shuffle(rng);
	}
}