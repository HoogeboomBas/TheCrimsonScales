using Fractural.Tasks;

public abstract class MasteryModel : AbstractModel, IEventSubscriber
{
	public abstract string Description { get; }

	public virtual bool SingleScenario => true;
	public virtual int MaxProgress => 1;

	public virtual async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
	{
		await GDTask.CompletedTask;
	}
}