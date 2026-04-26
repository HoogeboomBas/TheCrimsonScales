using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class DeviceSaveData : SaveData
{
	[JsonProperty]
	public Guid DeviceId { get; set; } = Guid.NewGuid();

	[JsonProperty]
	public int LastCampaignIndex { get; set; } = -1;

	[JsonProperty]
	public SavedDeviceOptions Options { get; set; } = new SavedDeviceOptions();
}