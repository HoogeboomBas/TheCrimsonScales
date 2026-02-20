using Godot;

public abstract partial class InfoItem<T> : InfoItemBase
	where T : InfoItemParameters
{
	protected ResizingLabel _titleLabel;

	protected T _parameters;

	public sealed override void Init(InfoItemParameters parameters)
	{
		if(parameters is not T castParameters)
		{
			Log.Error(
				$"Parameters are of type {parameters.GetType()}, but does not match the required type {typeof(T)}. Scene is {parameters.ScenePath}");
			return;
		}

		Init(castParameters);
	}

	public virtual void Init(T parameters)
	{
		_parameters = parameters;

		_titleLabel = GetNode<ResizingLabel>("MarginContainer/Content/LabelContainer/Label");
	}
}