using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class VoidEnergyIndicator : Node2D
{
	[Export]
	private Label _stackLabel;

	public void ShowAnimated()
	{
		Visible = true;

		float destinationScale = Scale.X;

		GTweenSequenceBuilder.New()
			.Append(this.TweenModulateAlpha(1f, 0.3f))
			.Join(this.TweenScale(0.8f, 0.3f))
			.Append(this.TweenScale(destinationScale, 0.2f)
				.SetEasing(Easing.OutBack))
			.Build()
			.PlayFastForwardable();
	}

	public void HideAnimated()
	{
		GTweenSequenceBuilder.New()
			.Append(this.TweenModulateAlpha(0f, 0.4f))
			.Join(this.TweenScale(1.3f, 0.4f)
				.SetEasing(Easing.InBack))
			.AppendCallback(() =>
			{
				Visible = false;
			})
			.Build()
			.PlayFastForwardable();
	}

	public void SetStackText(string text)
	{
		_stackLabel.Visible = text != null;
		_stackLabel.Text = text;
	}
}
