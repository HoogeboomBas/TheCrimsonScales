using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class BattleGoalProgressUpdateViewItem : Control
{
	private const float InitialPosX = 600f;

	[Export]
	private Control _container;
	[Export]
	private BattleGoalView _battleGoalView;
	[Export]
	private ProgressBar _progressBar;

	public void Init(BattleGoal battleGoal)
	{
		_battleGoalView.SetModel(battleGoal.Model);

		this.DelayedCall(() =>
		{
			_progressBar.Update(battleGoal.NormalizedProgress, $"{battleGoal.Progress}/{battleGoal.Model.MaxProgress}");
		});

		if(battleGoal.ProgressFull)
		{
			_progressBar.ProgressBarFill.SetSelfModulate(battleGoal.Model.FailIfProgressFull ? BattleGoal.FailedColor : BattleGoal.CompletedColor);
		}

		_container.SetPosition(new Vector2(InitialPosX, _container.Position.Y));

		GTweenSequenceBuilder.New()
			.Append(_container.TweenPositionX(0f, 0.8f).SetEasing(Easing.OutBack))
			.AppendTime(2f)
			.Append(_container.TweenPositionX(InitialPosX, 0.5f).SetEasing(Easing.InBack))
			.AppendCallback(QueueFree)
			.Build().Play();
	}
}