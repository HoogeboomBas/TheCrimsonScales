using Fractural.Tasks;

public abstract class FigureTrait
{
	protected Figure _figure;

	public FigureTrait AbstractModel { get; private set; }

	public FigureTrait ToMutable()
	{
		FigureTrait mutableClone = (FigureTrait)MemberwiseClone();
		mutableClone.DeepCloneFields();
		mutableClone.AbstractModel = this;
		return mutableClone;
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