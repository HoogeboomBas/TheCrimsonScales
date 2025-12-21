using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class PlayerParameters
{
	// [JsonProperty]
	// public PlayerColor PlayerColor { get; set; }

	[JsonProperty]
	public Guid PlayerDeviceGuid { get; set; }
}