using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedScenarioProgress
{
	[JsonProperty]
	public bool Discovered { get; set; }

	[JsonProperty]
	public bool Unlocked { get; set; }

	[JsonProperty]
	public bool Completed { get; set; }

	[JsonProperty]
	public List<int> CollectedTreasureChestNumbers { get; } = new List<int>();

	[JsonProperty]
	public Dictionary<string, object> CustomValues { get; } = new Dictionary<string, object>();
}