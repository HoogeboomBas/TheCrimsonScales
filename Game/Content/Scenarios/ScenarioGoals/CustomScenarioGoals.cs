using Fractural.Tasks;

public class CustomScenarioGoals : ScenarioGoals
{
	public override string Text { get; }

	public CustomScenarioGoals(string text)
	{
		Text = text;
	}

	public override void Start()
	{
	}

	public new async GDTask Win()
	{
		await base.Win();
	}

	public new async GDTask Lose()
	{
		await base.Lose();
	}
}