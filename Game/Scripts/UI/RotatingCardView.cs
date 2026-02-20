using System;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;

public partial class RotatingCardView : Control
{
	private static readonly StringName RotationName = "y_rot";

	public GTween GetRotationTween(Action flipSideCallback, float initialDelay = 0f)
	{
		return GTweenSequenceBuilder.New()
			.AppendTime(initialDelay)
			.Append(this.TweenInstanceShaderPropertyFloat(RotationName, 90f, 0.2f).SetEasing(Easing.Linear))
			.AppendCallback(() =>
			{
				flipSideCallback?.Invoke();
			})
			.Append(this.TweenInstanceShaderPropertyFloat(RotationName, -90f, 0f))
			.Append(this.TweenInstanceShaderPropertyFloat(RotationName, 0f, 0.5f).SetEasing(Easing.OutBack))
			.Build();
	}
}