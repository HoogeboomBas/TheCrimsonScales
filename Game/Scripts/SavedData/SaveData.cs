using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class SaveData
{
	[JsonProperty]
	public int MigrationVersion { get; set; }

	[JsonProperty]
	public string AppVersion { get; set; }

	[JsonProperty]
	public DateTime? LastSaved { get; set; }
}