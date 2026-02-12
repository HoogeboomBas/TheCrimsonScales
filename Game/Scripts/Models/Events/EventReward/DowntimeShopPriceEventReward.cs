using System;
using Godot;

public class DowntimeShopPriceEventReward(
	Func<EventReward, BetweenScenariosEvents.CalculateBuyPrice.ApplyFunction> calculatePriceApplyFunction,
	Func<EventReward, BetweenScenariosEvents.ItemBought.ApplyFunction> itemBoughtApplyFunction,
	Func<Color, string> getLabelText)
	: EventReward
{
	public override EventRewardType Type => EventRewardType.DuringDowntime;
	public override string GetLabelText(Color textColor) => getLabelText(textColor);

	public override void SubscribeDuringDowntime(SavedEventState savedEventState)
	{
		base.SubscribeDuringDowntime(savedEventState);

		BetweenScenariosEvents.CalculateBuyPriceEvent.Subscribe(this, calculatePriceApplyFunction(this));
		BetweenScenariosEvents.ItemBoughtEvent.Subscribe(this, itemBoughtApplyFunction(this));
	}

	public override void UnsubscribeDuringDowntime()
	{
		base.UnsubscribeDuringDowntime();

		BetweenScenariosEvents.CalculateBuyPriceEvent.Unsubscribe(this);
		BetweenScenariosEvents.ItemBoughtEvent.Unsubscribe(this);
	}
}