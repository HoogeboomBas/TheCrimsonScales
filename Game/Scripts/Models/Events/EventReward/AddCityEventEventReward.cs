using Fractural.Tasks;
using Godot;

public class AddCityEventEventReward(EventModel eventModel) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"City Event {eventModel.Number} is added to the City Event deck.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		BetweenScenariosController.Instance.SavedCampaign.SavedEvents.AddCityEventToDeck(eventModel, BetweenScenariosController.Instance.RNG);
	}
}