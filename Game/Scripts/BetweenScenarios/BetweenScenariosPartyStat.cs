using Godot;

public partial class BetweenScenariosPartyStat : Control
{
	[Export]
	private Control _progressBarContainer;
	[Export]
	private Control _progressBarFill;
	[Export]
	private Label _valueLabel;

	public void Update(float normalizedProgress, string valueLabelText)
	{
		_progressBarFill.SetSize(new Vector2(normalizedProgress * _progressBarContainer.Size.X, _progressBarFill.Size.Y));
		_valueLabel.SetText(valueLabelText);
	}
}