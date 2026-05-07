using System;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AddCityReward : SavedReward
{
	[JsonProperty]
	private string _eventModelId;

	private EventModel EventModel => ModelDB.GetById<EventModel>(_eventModelId);

	public override RewardType Type => RewardType.Immediate;

	public AddCityReward()
	{
	}

	public AddCityReward(EventModel eventModel)
	{
		_eventModelId = eventModel.Id.ToString();
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"City Event {EventModel.Number} is added to the City Event deck.";

	public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
	{
		await base.ImmediateResolve(savedCampaign, cancellationToken);

		savedCampaign.SavedEvents.AddCityEventToDeck(EventModel, BetweenScenariosController.Instance.RNG);
	}
}