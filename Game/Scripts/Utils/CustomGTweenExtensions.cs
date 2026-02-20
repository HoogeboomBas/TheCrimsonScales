using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Extensions;
using GTweens.Tweeners;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public static class CustomGTweenExtensions
{
	public static async GDTask PlayFastForwardableAsync(this GTween gTween)
	{
		await gTween.PlayAsync(GameController.FastForward, GameController.CancellationToken);
	}

	public static GTween PlayFastForwardable(this GTween gTween)
	{
		return gTween.Play(GameController.FastForward);
	}

	public static GTween TweenScale(this Node3D target, float to, float duration)
	{
		return target.TweenScale(to * Vector3.One, duration);
	}

	public static GTween TweenScale(this Node2D target, float to, float duration)
	{
		return target.TweenScale(to * Vector2.One, duration);
	}

	public static GTween TweenScale(this Control target, float to, float duration)
	{
		return target.TweenScale(to * Vector2.One, duration);
	}

	// public static GTween TweenPulse(this Node3D target, float totalDuration, float pulseAmount = 1.2f)
	// {
	// 	return GTweenSequenceBuilder.New()
	// 		.Append(target.TweenScale(pulseAmount, 0.5f * totalDuration).SetEasing(Easing.OutQuad))
	// 		.Append(target.TweenScale(1f, 0.5f * totalDuration).SetEasing(Easing.InQuad)).Build();
	// }
	//
	// public static GTween TweenPulse(this Control target, float totalDuration, float pulseAmount = 1.2f)
	// {
	// 	return GTweenSequenceBuilder.New()
	// 		.Append(target.TweenScale(pulseAmount, 0.5f * totalDuration).SetEasing(Easing.OutQuad))
	// 		.Append(target.TweenScale(1f, 0.5f * totalDuration).SetEasing(Easing.InQuad)).Build();
	// }

	public static GTween TweenProgress(this PathFollow3D target, float to, float duration)
	{
		return GTweenExtensions.Tween(
			() => target.Progress,
			current => target.Progress = current,
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenProgressRatio(this PathFollow3D target, float to, float duration)
	{
		return GTweenExtensions.Tween(
			() => target.ProgressRatio,
			current => target.ProgressRatio = current,
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween TweenGlobalJump(this Node2D target, Vector2 to, float jumpHeight, float duration)
	{
		GTween yTween = GTweenSequenceBuilder.New()
			.Append(target.TweenGlobalPositionY((target.GlobalPosition.Y + to.Y) * 0.5f - jumpHeight, duration * 0.5f).SetEasing(Easing.OutQuad))
			.Append(target.TweenGlobalPositionY(to.Y, duration * 0.5f).SetEasing(Easing.InQuad))
			.Build();

		return GTweenSequenceBuilder.New()
			//.Join(target.TweenGlobalPosition(to, duration))
			.Join(target.TweenGlobalPositionX(to.X, duration).SetEasing(Easing.Linear))
			.Join(yTween)
			.Build();
	}

	public static GTween TweenGlobalJump(this Node3D target, Vector3 to, float jumpHeight, float duration)
	{
		GTween yTween = GTweenSequenceBuilder.New()
			.Append(target.TweenGlobalPositionY((target.GlobalPosition.Y + to.Y) * 0.5f + jumpHeight, duration * 0.5f).SetEasing(Easing.OutQuad))
			.Append(target.TweenGlobalPositionY(to.Y, duration * 0.5f).SetEasing(Easing.InQuad))
			.Build();

		return GTweenSequenceBuilder.New()
			.Join(target.TweenGlobalPositionXZ(to.XZ(), duration))
			.Join(yTween)
			.Build();
	}

	public static GTween TweenJump(this Node3D target, Vector3 to, float jumpHeight, float duration)
	{
		GTween yTween = GTweenSequenceBuilder.New()
			.Append(target.TweenPositionY((target.Position.Y + to.Y) * 0.5f + jumpHeight, duration * 0.5f).SetEasing(Easing.OutQuad))
			.Append(target.TweenPositionY(to.Y, duration * 0.5f).SetEasing(Easing.InQuad))
			.Build();

		return GTweenSequenceBuilder.New()
			.Join(target.TweenPositionXZ(to.XZ(), duration))
			.Join(yTween)
			.Build();
	}

	public static GTween TweenPulse(this Node2D target, float targetScale, float duration)
	{
		return GTweenSequenceBuilder.New()
			.Append(target.TweenScale(targetScale, duration * 0.5f).SetEasing(Easing.OutQuad))
			.Append(target.TweenScale(1f, duration * 0.5f).SetEasing(Easing.OutBack))
			.Build();
	}

	public static GTween TweenPulse(this Control target, float targetScale, float duration)
	{
		return GTweenSequenceBuilder.New()
			.Append(target.TweenScale(targetScale, duration * 0.5f).SetEasing(Easing.OutQuad))
			.Append(target.TweenScale(1f, duration * 0.5f).SetEasing(Easing.OutBack))
			.Build();
	}

	public static GTween TweenValue(this TextureProgressBar target, float to, float duration)
	{
		return GTweenExtensions.Tween(
			() => (float)target.Value,
			current => target.Value = current,
			to,
			duration,
			GodotObjectExtensions.GetGodotObjectValidationFunction(target)
		);
	}

	public static GTween Tween(Tweener<float>.Setter setter, float duration)
	{
		return GTweenExtensions.Tween(() => 0f, setter, 1f, duration);
	}

	public static GTween Tween(float from, float to, Tweener<float>.Setter setter, float duration)
	{
		return GTweenExtensions.Tween(() => from, setter, to, duration);
	}
}