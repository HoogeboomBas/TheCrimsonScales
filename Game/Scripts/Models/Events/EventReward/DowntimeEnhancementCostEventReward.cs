using System;
using Godot;

public class DowntimeEnhancementCostEventReward(
	Func<EventReward, BetweenScenariosEvents.CalculateEnhancementCost.ApplyFunction> calculateCostApplyFunction,
	Func<EventReward, BetweenScenariosEvents.EnhancementBought.ApplyFunction> enhancementBoughtApplyFunction,
	Func<Color, string> getLabelText)
	: EventReward
{
	public override EventRewardType Type => EventRewardType.DuringDowntime;
	public override string GetLabelText(Color textColor) => getLabelText(textColor);

	public override void SubscribeDuringDowntime(SavedEventState savedEventState)
	{
		base.SubscribeDuringDowntime(savedEventState);

		BetweenScenariosEvents.CalculateEnhancementCostEvent.Subscribe(this, calculateCostApplyFunction(this));
		BetweenScenariosEvents.EnhancementBoughtEvent.Subscribe(this, enhancementBoughtApplyFunction(this));
	}

	public override void UnsubscribeDuringDowntime()
	{
		base.UnsubscribeDuringDowntime();

		BetweenScenariosEvents.CalculateEnhancementCostEvent.Unsubscribe(this);
		BetweenScenariosEvents.EnhancementBoughtEvent.Unsubscribe(this);
	}
}