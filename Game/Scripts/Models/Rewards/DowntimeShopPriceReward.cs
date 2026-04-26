using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class DowntimeShopPriceReward : SavedReward
{
	public override RewardType Type => RewardType.DuringDowntime;

	public override void SubscribeDuringDowntime()
	{
		base.SubscribeDuringDowntime();

		BetweenScenariosEvents.CalculateItemBuyPriceEvent.Subscribe(this, CalculatePriceApplyFunction);
		BetweenScenariosEvents.ItemBoughtEvent.Subscribe(this, ItemBoughtApplyFunction);
	}

	public override void UnsubscribeDuringDowntime()
	{
		base.UnsubscribeDuringDowntime();

		BetweenScenariosEvents.CalculateItemBuyPriceEvent.Unsubscribe(this);
		BetweenScenariosEvents.ItemBoughtEvent.Unsubscribe(this);
	}

	protected abstract void CalculatePriceApplyFunction(BetweenScenariosEvents.CalculateItemBuyPrice.Parameters parameters);

	protected abstract void ItemBoughtApplyFunction(BetweenScenariosEvents.ItemBought.Parameters parameters);
}