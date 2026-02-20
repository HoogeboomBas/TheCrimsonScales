using Fractural.Tasks;
using Godot;

public abstract class EventReward
{
	public abstract EventRewardType Type { get; }
	public abstract string GetLabelText(Color textColor);

	public virtual async GDTask ImmediateResolve()
	{
		await GDTask.CompletedTask;
	}

	public virtual void SubscribeDuringDowntime(SavedEventState savedEventState)
	{
	}

	public virtual void UnsubscribeDuringDowntime()
	{
	}

	public virtual async GDTask OnScenarioSetupPhaseCompleted()
	{
		await GDTask.CompletedTask;
	}
}