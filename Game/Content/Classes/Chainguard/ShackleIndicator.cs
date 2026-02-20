using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class ShackleIndicator : Node2D
{
	public void Init()
	{
		if(!GameController.FastForward)
		{
			float destinationScale = Scale.X;

			this.TweenModulateAlpha(0f, 0f).Play(true);

			SetScale(Vector2.One * 1.3f);

			GTweenSequenceBuilder.New()
				.Append(this.TweenModulateAlpha(1f, 0.3f))
				.Join(this.TweenScale(0.8f, 0.3f))
				.Append(this.TweenScale(destinationScale, 0.2f).SetEasing(Easing.OutBack))
				.Build().PlayFastForwardable();
		}
	}

	public void Destroy()
	{
		GTweenSequenceBuilder.New()
			.Append(this.TweenModulateAlpha(0f, 0.4f))
			.Join(this.TweenScale(1.3f, 0.4f).SetEasing(Easing.InBack))
			.AppendCallback(QueueFree)
			.Build().PlayFastForwardable();
	}

	public void Flash()
	{
		if(!GameController.FastForward)
		{
			float destinationScale = Scale.X;

			GTweenSequenceBuilder.New()
				.Append(this.TweenScale(0.8f, 0.2f))
				.Append(this.TweenScale(destinationScale, 0.2f).SetEasing(Easing.OutBack))
				.Build().PlayFastForwardable();
		}
	}
}