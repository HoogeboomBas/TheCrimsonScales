using System;
using Fractural.Tasks;
using Godot;

public class OnScenarioStartedEventReward(Func<GDTask> onScenarioSetupPhaseCompleted, Func<Color, string> labelText) : EventReward
{
	public override EventRewardType Type => EventRewardType.ScenarioStart;
	public override string GetLabelText(Color textColor) => labelText(textColor);

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		if(onScenarioSetupPhaseCompleted != null)
		{
			await onScenarioSetupPhaseCompleted();
		}
	}
}