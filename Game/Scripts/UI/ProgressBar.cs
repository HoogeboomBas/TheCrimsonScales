using Godot;

public partial class ProgressBar : Control
{
	[Export]
	private Control _progressBarContainer;
	[Export]
	public Control ProgressBarFill;
	[Export]
	private Label _valueLabel;

	public void Update(float normalizedProgress, string valueLabelText)
	{
		normalizedProgress = Mathf.Clamp(normalizedProgress, 0f, 1f);
		ProgressBarFill.SetSize(new Vector2(normalizedProgress * _progressBarContainer.Size.X, ProgressBarFill.Size.Y));
		_valueLabel.SetText(valueLabelText);
	}
}