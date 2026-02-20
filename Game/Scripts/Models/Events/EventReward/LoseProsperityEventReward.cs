using Fractural.Tasks;
using Godot;

public class LoseProsperityEventReward(int prosperity) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Lose {prosperity} prosperity.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		BetweenScenariosController.Instance.SavedCampaign.AdjustProsperity(-prosperity);
	}
}