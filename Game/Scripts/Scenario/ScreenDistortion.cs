using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ScreenDistortion : FullScreenControl
{
	private static readonly StringName LensCenter1Name = "lens_center_1";
	private static readonly StringName LensRadius1Name = "lens_radius_1";
	private static readonly StringName LensPower1Name = "lens_power_1";
	private static readonly StringName LensCenter2Name = "lens_center_2";
	private static readonly StringName LensRadius2Name = "lens_radius_2";
	private static readonly StringName LensPower2Name = "lens_power_2";
	private static readonly StringName CameraZoomName = "camera_zoom";

	private ShaderMaterial _shaderMaterial;
	private Node2D _target1;
	private Node2D _target2;

	public override void _Ready()
	{
		base._Ready();

		_shaderMaterial = (ShaderMaterial)Material;

		SetPower(1, true);
		SetPower(1, false);

		SetVisible(false);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if(_target1 == null && _target2 == null)
		{
			return;
		}

		Camera2D camera = GameController.Instance.CameraController.Camera;
		if(_target1 != null)
		{
			Vector2 viewportPosition1 = _target1.GetGlobalTransformWithCanvas().Origin / GetViewport().GetVisibleRect().Size;
			_shaderMaterial.SetShaderParameter(LensCenter1Name, viewportPosition1);
		}

		if(_target2 != null)
		{
			Vector2 viewportPosition2 = _target2.GetGlobalTransformWithCanvas().Origin / GetViewport().GetVisibleRect().Size;
			_shaderMaterial.SetShaderParameter(LensCenter2Name, viewportPosition2);
		}

		_shaderMaterial.SetShaderParameter(CameraZoomName, camera.Zoom.X);
	}

	public void SetTarget(Node2D target, bool lens1)
	{
		if(lens1)
		{
			_target1 = target;
		}
		else
		{
			_target2 = target;
		}

		SetVisible(_target1 != null || _target2 != null);
	}

	public void SetPower(float power, bool lens1)
	{
		_shaderMaterial.SetShaderParameter(lens1 ? LensPower1Name : LensPower2Name, power);
	}

	public void SetRadius(float radius, bool lens1)
	{
		_shaderMaterial.SetShaderParameter(lens1 ? LensRadius1Name : LensRadius2Name, radius);
	}

	public GTween TweenPower(float to, float duration, bool lens1)
	{
		return _shaderMaterial.TweenPropertyFloat(lens1 ? LensPower1Name : LensPower2Name, to, duration);
	}

	public GTween TweenRadius(float to, float duration, bool lens1)
	{
		return _shaderMaterial.TweenPropertyFloat(lens1 ? LensRadius1Name : LensRadius2Name, to, duration);
	}

	public GTween Disappear(Node2D target, float animationSpeed, bool lens1)
	{
		const float radius = 0.7f;

		return GTweenSequenceBuilder.New()
			.AppendCallback(() =>
			{
				SetTarget(target, lens1);
				SetPower(1f, lens1);
				SetRadius(0.4f * radius, lens1);

				AppController.Instance.AudioController.Play("res://Audio/SFX/WHOOSH_Steam_Fast_01_mono.wav", 0.9f, 1.1f, delay: 0.0f);
			})
			.Append(TweenPower(1.1f, 0.2f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Append(TweenPower(0.4f, 0.5f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.3f * radius, 0.5f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Join(target.TweenScale(0f, 0.5f / animationSpeed).SetEasing(Easing.Linear))
			.Append(TweenPower(1f, 0.5f / animationSpeed, lens1).SetEasing(Easing.OutBack))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.AppendCallback(() =>
			{
				SetTarget(null, lens1);
			})
			.Build();
	}

	public GTween Appear(Node2D target, float animationSpeed, bool lens1)
	{
		const float radius = 0.7f;

		return GTweenSequenceBuilder.New()
			.AppendCallback(() =>
			{
				SetTarget(target, lens1);
				SetPower(1f, lens1);
				SetRadius(0.4f * radius, lens1);

				AppController.Instance.AudioController.Play("res://Audio/SFX/WHOOSH_Steam_Fast_01_mono.wav", 0.9f, 1.1f, delay: 0.0f);
			})
			.Append(TweenPower(1.1f, 0.2f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Append(TweenPower(0.4f, 0.5f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.3f * radius, 0.5f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Append(TweenPower(1f, 0.5f / animationSpeed, lens1).SetEasing(Easing.OutBack))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, lens1).SetEasing(Easing.OutCubic))
			.Join(target.TweenScale(1f, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
			.AppendCallback(() =>
			{
				SetTarget(null, lens1);
			})
			.Build();
	}

	public GTween Swap(Node2D targetA, Node2D targetB, float animationSpeed)
	{
		const float radius = 0.7f;

		return GTweenSequenceBuilder.New()
			.AppendCallback(() =>
			{
				SetTarget(targetA, true);
				SetTarget(targetB, false);
				SetPower(1f, true);
				SetPower(1f, false);
				SetRadius(0.4f * radius, true);
				SetRadius(0.4f * radius, false);

				AppController.Instance.AudioController.Play("res://Audio/SFX/WHOOSH_Steam_Fast_01_mono.wav", 0.9f, 1.1f, delay: 0.0f);
			})
			.Append(TweenPower(1.1f, 0.2f / animationSpeed, true).SetEasing(Easing.OutCubic))
			.Join(TweenPower(1.1f, 0.2f / animationSpeed, false).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, true).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, false).SetEasing(Easing.OutCubic))
			.Append(TweenPower(0.4f, 0.5f / animationSpeed, true).SetEasing(Easing.OutCubic))
			.Join(TweenPower(0.4f, 0.5f / animationSpeed, false).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.3f * radius, 0.5f / animationSpeed, true).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.3f * radius, 0.5f / animationSpeed, false).SetEasing(Easing.OutCubic))
			.Join(targetA.TweenScale(0f, 0.5f / animationSpeed).SetEasing(Easing.Linear))
			.Join(targetB.TweenScale(0f, 0.5f / animationSpeed).SetEasing(Easing.Linear))
			.AppendCallback(() =>
			{
				Vector2 tempPos = targetA.GlobalPosition;
				targetA.SetGlobalPosition(targetB.GlobalPosition);
				targetB.SetGlobalPosition(tempPos);
			})
			.Append(TweenPower(1f, 0.5f / animationSpeed, true).SetEasing(Easing.OutBack))
			.Join(TweenPower(1f, 0.5f / animationSpeed, false).SetEasing(Easing.OutBack))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, true).SetEasing(Easing.OutCubic))
			.Join(TweenRadius(0.4f * radius, 0.2f / animationSpeed, false).SetEasing(Easing.OutCubic))
			.Join(targetA.TweenScale(1f, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
			.Join(targetB.TweenScale(1f, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
			.AppendCallback(() =>
			{
				SetTarget(null, true);
				SetTarget(null, false);
			})
			.Build();
	}
}