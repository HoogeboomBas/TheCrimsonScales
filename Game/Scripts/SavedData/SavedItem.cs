using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedItem
{
	[JsonProperty]
	public int UnlockedCount { get; set; }

	[JsonProperty]
	public int StockCount { get; set; }

	public event Action<SavedItem> StockCountChangedEvent;

	public void AddUnlocked(int count)
	{
		UnlockedCount += count;
	}

	public void RemovedUnlocked(int count)
	{
		UnlockedCount -= count;
	}

	public void AddStock(int count)
	{
		StockCount += count;

		StockCountChangedEvent?.Invoke(this);
	}

	public void RemoveStock(int count)
	{
		StockCount -= count;

		StockCountChangedEvent?.Invoke(this);
	}
}