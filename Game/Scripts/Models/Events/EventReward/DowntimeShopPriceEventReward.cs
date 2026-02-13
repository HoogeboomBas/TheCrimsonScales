using System;
using Godot;

public class DowntimeShopPriceEventReward(
	Func<EventReward, BetweenScenariosEvents.CalculateItemBuyPrice.ApplyFunction> calculatePriceApplyFunction,
	Func<EventReward, BetweenScenariosEvents.ItemBought.ApplyFunction> itemBoughtApplyFunction,
	Func<Color, string> getLabelText)
	: EventReward
{
	public override EventRewardType Type => EventRewardType.DuringDowntime;
	public override string GetLabelText(Color textColor) => getLabelText(textColor);

	public override void SubscribeDuringDowntime(SavedEventState savedEventState)
	{
		base.SubscribeDuringDowntime(savedEventState);

		BetweenScenariosEvents.CalculateItemBuyPriceEvent.Subscribe(this, calculatePriceApplyFunction(this));
		BetweenScenariosEvents.ItemBoughtEvent.Subscribe(this, itemBoughtApplyFunction(this));
	}

	public override void UnsubscribeDuringDowntime()
	{
		base.UnsubscribeDuringDowntime();

		BetweenScenariosEvents.CalculateItemBuyPriceEvent.Unsubscribe(this);
		BetweenScenariosEvents.ItemBoughtEvent.Unsubscribe(this);
	}
}