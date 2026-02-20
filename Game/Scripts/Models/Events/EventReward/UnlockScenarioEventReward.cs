using Fractural.Tasks;
using Godot;

public class UnlockScenarioEventReward(ScenarioModel scenarioModel) : EventReward
{
	public override EventRewardType Type => EventRewardType.Immediate;
	public override string GetLabelText(Color textColor) => $"Unlock Scenario {scenarioModel.ScenarioNumber}.";

	public override async GDTask ImmediateResolve()
	{
		await base.ImmediateResolve();

		BetweenScenariosController.Instance.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(scenarioModel).Discover();
	}
}