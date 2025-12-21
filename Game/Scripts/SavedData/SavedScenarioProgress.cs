using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedScenarioProgress
{
	[JsonProperty]
	public bool Discovered { get; private set; }

	[JsonProperty]
	public bool ShownOnMap { get; private set; }

	// [JsonProperty]
	// public bool Unlocked { get; set; }

	[JsonProperty]
	public bool Completed { get; private set; }

	[JsonProperty]
	public List<int> CollectedTreasureChestNumbers { get; private set; } = new List<int>();

	[JsonProperty]
	public Dictionary<string, object> CustomValues { get; private set; } = new Dictionary<string, object>();

	public void Discover()
	{
		Discovered = true;
	}

	public void ShowOnMap()
	{
		ShownOnMap = true;
	}

	public void Complete()
	{
		Completed = true;
	}
}