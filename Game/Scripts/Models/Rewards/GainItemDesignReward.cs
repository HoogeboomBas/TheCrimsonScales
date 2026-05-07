using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class GainItemDesignReward : SavedReward
{
	[JsonProperty]
	private string _itemModelId;

	private ItemModel ItemModel => ModelDB.GetById<ItemModel>(_itemModelId);

	public override RewardType Type => RewardType.Immediate;

	public GainItemDesignReward()
	{
	}

	public GainItemDesignReward(ItemModel itemModel)
	{
		_itemModelId = itemModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Gain '{ItemModel.Name}' item design.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		SavedItem savedItem = savedCampaign.GetSavedItem(ItemModel);
		savedItem.AddUnlocked(1);
		savedItem.AddStock(1);
	}
}