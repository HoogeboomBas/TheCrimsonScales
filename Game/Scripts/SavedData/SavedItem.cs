using System;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedItem
{
	[JsonProperty]
	public string ItemModelId { get; private set; }

	[JsonProperty]
	public int UnlockedCount { get; private set; }

	[JsonProperty]
	public int StockCount { get; private set; }

	public ItemModel ItemModel => ModelDB.GetById<ItemModel>(ItemModelId);

	public event Action<SavedItem> StockCountChangedEvent;

	public SavedItem()
	{
	}

	public SavedItem(ItemModel itemModel)
	{
		ItemModelId = itemModel.Id.ToString();
	}

	public void AddUnlocked(int count)
	{
		UnlockedCount += count;
		UnlockedCount = Mathf.Min(UnlockedCount, ItemModel.ShopCount);
	}

	public void RemovedUnlocked(int count)
	{
		UnlockedCount -= count;
		UnlockedCount = Mathf.Max(UnlockedCount, 0);
	}

	public void AddStock(int count)
	{
		StockCount += count;
		StockCount = Mathf.Min(StockCount, UnlockedCount);

		StockCountChangedEvent?.Invoke(this);
	}

	public void RemoveStock(int count)
	{
		StockCount -= count;
		StockCount = Mathf.Max(StockCount, 0);

		StockCountChangedEvent?.Invoke(this);
	}
}