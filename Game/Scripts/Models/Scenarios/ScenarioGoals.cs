using Fractural.Tasks;

public abstract class ScenarioGoals : IEventSubscriber
{
	public abstract string Text { get; }

	public abstract void Start();

	protected async GDTask Win()
	{
		await AbilityCmd.Win();
	}

	protected async GDTask Lose()
	{
		await AbilityCmd.Lose();
	}
}