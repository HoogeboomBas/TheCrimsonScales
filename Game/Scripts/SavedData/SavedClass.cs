using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedClass
{
	// Whether the class has been unlocked
	[JsonProperty]
	public bool Unlocked { get; set; }

	// Whether the class has been retired at some point
	[JsonProperty]
	public bool Retired { get; set; }

	public void Unlock()
	{
		Unlocked = true;
	}
}