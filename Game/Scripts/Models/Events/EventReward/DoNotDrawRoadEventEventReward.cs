using Godot;

public class DoNotDrawRoadEventEventReward() : EventReward
{
	public override EventRewardType Type => EventRewardType.DuringDowntime;
	public override string GetLabelText(Color textColor) => "Do not draw a road event.";

	public override void SubscribeDuringDowntime(SavedEventState savedEventState)
	{
		base.SubscribeDuringDowntime(savedEventState);

		BetweenScenariosEvents.DrawRoadEventEvent.Subscribe(this,
			parameters =>
			{
				parameters.SetDrawEvent(false);
				savedEventState.Complete(this);
			}
		);
	}

	public override void UnsubscribeDuringDowntime()
	{
		base.UnsubscribeDuringDowntime();

		BetweenScenariosEvents.DrawRoadEventEvent.Unsubscribe(this);
	}
}