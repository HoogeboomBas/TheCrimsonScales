using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ScreenDistortion : FullScreenControl
{
	private static readonly StringName LensCenterName = "lens_center";
	private static readonly StringName LensRadiusName = "lens_radius";
	private static readonly StringName LensPowerName = "lens_power";
	private static readonly StringName CameraZoomName = "camera_zoom";

	private ShaderMaterial _shaderMaterial;
	private Node2D _target;

	public override void _Ready()
	{
		base._Ready();

		_shaderMaterial = (ShaderMaterial)Material;

		SetVisible(false);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if(_target == null)
		{
			return;
		}

		Camera2D camera = GameController.Instance.CameraController.Camera;
		Vector2 viewportPosition = _target.GetGlobalTransformWithCanvas().Origin / GetViewport().GetVisibleRect().Size;
		_shaderMaterial.SetShaderParameter(LensCenterName, viewportPosition);
		_shaderMaterial.SetShaderParameter(CameraZoomName, camera.Zoom.X);
	}

	public void Open(Node2D target)
	{
		_target = target;
		SetVisible(true);
	}

	public void Close()
	{
		_target = null;
		SetVisible(false);
	}

	public void SetPower(float power)
	{
		_shaderMaterial.SetShaderParameter(LensPowerName, power);
	}

	public void SetRadius(float radius)
	{
		_shaderMaterial.SetShaderParameter(LensRadiusName, radius);
	}

	public GTween TweenPower(float to, float duration)
	{
		return _shaderMaterial.TweenPropertyFloat(LensPowerName, to, duration);
	}

	public GTween TweenRadius(float to, float duration)
	{
		return _shaderMaterial.TweenPropertyFloat(LensRadiusName, to, duration);
	}
}