using System;
using Godot;

public class DowntimeShopSellPriceEventReward(
	Func<EventReward, BetweenScenariosEvents.CalculateSellPrice.ApplyFunction> calculatePriceApplyFunction,
	Func<EventReward, BetweenScenariosEvents.ItemSold.ApplyFunction> itemSoldApplyFunction,
	Func<Color, string> getLabelText)
	: EventReward
{
	public override EventRewardType Type => EventRewardType.DuringDowntime;
	public override string GetLabelText(Color textColor) => getLabelText(textColor);

	public override void SubscribeDuringDowntime(SavedEventState savedEventState)
	{
		base.SubscribeDuringDowntime(savedEventState);

		BetweenScenariosEvents.CalculateSellPriceEvent.Subscribe(this, calculatePriceApplyFunction(this));
		BetweenScenariosEvents.ItemSoldEvent.Subscribe(this, itemSoldApplyFunction(this));
	}

	public override void UnsubscribeDuringDowntime()
	{
		base.UnsubscribeDuringDowntime();

		BetweenScenariosEvents.CalculateSellPriceEvent.Unsubscribe(this);
		BetweenScenariosEvents.ItemSoldEvent.Unsubscribe(this);
	}
}