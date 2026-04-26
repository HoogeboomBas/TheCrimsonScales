using Godot;

public partial class GoalsViewGoal : Control
{
	[Export]
	private RichTextLabel _label;
	[Export]
	private ProgressBar _progressBar;

	public ScenarioGoal Goal { get; private set; }

	public void Init(ScenarioGoal goal)
	{
		Goal = goal;

		RichTextParameters textParameters = _label.GetRichTextParameters();
		_label.SetText(Goal.GetLabelText(textParameters));

		Goal.ProgressUpdatedEvent += OnProgressUpdated;

		UpdateProgress();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(Goal != null)
		{
			Goal.ProgressUpdatedEvent -= OnProgressUpdated;
		}
	}

	private void UpdateProgress()
	{
		_progressBar.SetVisible(Goal.HasProgress);

		if(Goal.HasProgress)
		{
			float normalizedProgress = 0f;
			if(Goal.MaxProgress.HasValue)
			{
				normalizedProgress = (float)Goal.Progress / Goal.MaxProgress.Value;
			}

			this.DelayedCall(() =>
			{
				_progressBar.Update(normalizedProgress, $"{Goal.Progress}/{(Goal.MaxProgress.HasValue ? Goal.MaxProgress.Value : "?")}");
			});
		}
	}

	private void OnProgressUpdated(ScenarioGoal goal)
	{
		UpdateProgress();
	}
}