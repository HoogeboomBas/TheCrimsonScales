using Fractural.Tasks;
using Godot;

public class AddRoadEventEventReward(EventModel eventModel) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Road Event {eventModel.Number} is added to the Road Event deck.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		BetweenScenariosController.Instance.SavedCampaign.SavedEvents.AddRoadEventToDeck(eventModel, BetweenScenariosController.Instance.RNG);
	}
}