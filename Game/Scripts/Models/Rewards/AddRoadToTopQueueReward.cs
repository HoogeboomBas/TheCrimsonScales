using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AddRoadToTopQueueReward : SavedReward
{
	[JsonProperty]
	private string _eventModelId;

	private EventModel EventModel => ModelDB.GetById<EventModel>(_eventModelId);

	public override RewardType Type => RewardType.Immediate;

	public AddRoadToTopQueueReward()
	{
	}

	public AddRoadToTopQueueReward(EventModel eventModel)
	{
		_eventModelId = eventModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"Road Event {EventModel.Number} is added to the top of the Road Event deck.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		savedCampaign.SavedEvents.AddRoadEventToTopQueue(EventModel);
	}
}