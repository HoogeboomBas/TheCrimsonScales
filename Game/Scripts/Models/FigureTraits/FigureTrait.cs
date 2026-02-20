using Fractural.Tasks;

public abstract class FigureTrait
{
	protected Figure _figure;

	public FigureTrait ToMutable()
	{
		FigureTrait abstractModel = (FigureTrait)MemberwiseClone();
		abstractModel.DeepCloneFields();
		return abstractModel;
	}

	public virtual async GDTask Activate(Figure figure)
	{
		_figure = figure;

		await GDTask.CompletedTask;
	}

	public virtual async GDTask Deactivate(Figure figure)
	{
		await GDTask.CompletedTask;
	}

	protected virtual void DeepCloneFields()
	{
	}
}