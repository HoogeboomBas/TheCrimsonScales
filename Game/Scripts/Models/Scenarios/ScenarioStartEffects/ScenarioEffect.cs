using Fractural.Tasks;

public abstract class ScenarioEffect
{
	public abstract string GetDescription(RichTextParameters textParameters);

	public abstract GDTask Activate(Character character);
}