public class ScenarioLink<T> : ScenarioLink
	where T : ScenarioModel
{
	public override bool ToGloomhaven => false;

	public ScenarioLink()
		: base(ModelDB.Scenario<T>())
	{
	}
}

public abstract class ScenarioLink
{
	public ScenarioModel To { get; }
	public abstract bool ToGloomhaven { get; }

	protected ScenarioLink(ScenarioModel to)
	{
		To = to;
	}
}