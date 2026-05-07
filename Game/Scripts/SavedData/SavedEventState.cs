using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedEventState
{
	[JsonProperty]
	public string ChoiceId { get; private set; }

	[JsonProperty]
	public Dictionary<string, object> CustomValues { get; private set; } = new Dictionary<string, object>();

	public bool Completed { get; private set; }

	public EventChoiceModel Choice => ModelDB.GetById<EventChoiceModel>(ChoiceId);

	public SavedEventState()
	{
	}

	public SavedEventState(EventChoiceModel choice)
	{
		ChoiceId = choice.Id.ToString();
	}

	public void SetCustomValue(string key, object value)
	{
		CustomValues[key] = value;
	}

	public T GetCustomValue<T>(string key)
	{
		if(!CustomValues.TryGetValue(key, out object value))
		{
			//Log.Error($"Could not find custom value for key: {key}");
			return default;
		}

		if(value is not T castValue)
		{
			Log.Error($"Could not cast custom value for key: {key}");
			return default;
		}

		return castValue;
	}

	public bool TryGetCustomValue<T>(string key, out T value)
	{
		if(!CustomValues.TryGetValue(key, out object retrievedValue))
		{
			//Log.Error($"Could not find custom value for: {source} with key: {key}");
			value = default;
			return false;
		}

		if(retrievedValue is not T castValue)
		{
			Log.Error($"Could not cast custom value for key: {key}");
			value = default;
			return false;
		}

		value = castValue;
		return true;
	}
}