using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class GoldDistributionPopupButton : Control
{
	[Export]
	private Control _scaleContainer;

	[Export]
	public BetterButton Button { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		//_scaleContainer.SetPivotOffset(_scaleContainer.Size * 0.5f);
		//_scaleContainer.SetScale(Vector2.Zero);
	}

	public void SetActive(bool active)
	{
		Button.SetEnabled(active, true);
		// if(active)
		// {
		// 	_scaleContainer.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).Play();
		// }
		// else
		// {
		// 	_scaleContainer.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).Play();
		// }
	}
}