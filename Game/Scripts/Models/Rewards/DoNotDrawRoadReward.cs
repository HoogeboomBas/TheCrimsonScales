using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class DoNotDrawRoadReward : SavedReward
{
	public override RewardType Type => RewardType.DuringDowntime;

	public DoNotDrawRoadReward()
	{
	}

	public override string GetLabelText(RichTextParameters textParameters) => "Do not draw a road event.";

	public override void SubscribeDuringDowntime()
	{
		base.SubscribeDuringDowntime();

		BetweenScenariosEvents.DrawRoadEventEvent.Subscribe(this,
			parameters =>
			{
				parameters.SetDrawEvent(false);
				Complete();
			}
		);
	}

	public override void UnsubscribeDuringDowntime()
	{
		base.UnsubscribeDuringDowntime();

		BetweenScenariosEvents.DrawRoadEventEvent.Unsubscribe(this);
	}
}