using Fractural.Tasks;
using Godot;

public class LoseReputationEventReward(int reputationAmount) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Lose {reputationAmount} reputation.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		BetweenScenariosController.Instance.SavedCampaign.AdjustReputation(-reputationAmount);
	}
}