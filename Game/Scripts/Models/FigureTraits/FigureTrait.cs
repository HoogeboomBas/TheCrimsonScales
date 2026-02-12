using Fractural.Tasks;

public abstract class FigureTrait
{
	public virtual async GDTask Activate(Figure figure)
	{
		await GDTask.CompletedTask;
	}

	public virtual async GDTask Deactivate(Figure figure)
	{
		await GDTask.CompletedTask;
	}
}