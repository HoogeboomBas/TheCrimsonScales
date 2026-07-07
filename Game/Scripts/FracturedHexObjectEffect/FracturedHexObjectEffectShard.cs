using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class FracturedHexObjectEffectShard : Node2D
{
	[Export]
	private Node2D _offset;
	[Export]
	private Polygon2D _polygon;

	private Vector2 _velocity;
	private float _angularVelocity;
	private float _dissolveProgress;

	public void Init(Vector2[] polygon, Texture2D texture, Vector2 textureSize)
	{
		_polygon.SetPolygon(polygon);
		_polygon.SetUV(polygon);
		_polygon.SetTexture(texture);

		Vector2 center = Vector2.Zero;

		foreach(var p in polygon)
		{
			center += p;
		}

		center /= polygon.Length;

		_offset.SetPosition(center);
		_polygon.SetPosition(-center);

		Vector2 direction = center - textureSize / 2;

		if(direction.Length() < 1)
		{
			direction = Vector2.Right;
		}

		// direction = direction.Normalized();
		direction = direction.Normalized() * Mathf.Lerp(direction.Length() * 0.01f, 1, 0.7f);

		this.DelayedCall(() =>
		{
			_velocity = direction * Mathf.Lerp(300f, 600f, GD.Randf());
			_angularVelocity = Mathf.Lerp(-3f, 3f, GD.Randf());

			GTweenSequenceBuilder.New()
				.AppendTime(Mathf.Lerp(0.0f, 0.15f, GD.Randf()))
				.Append(_polygon.TweenInstanceShaderPropertyFloat("progress", 1f, Mathf.Lerp(0.2f, 0.4f, GD.Randf())))
				.Build().Play();
			// GTweenSequenceBuilder.New()
			// 	.AppendTime(Mathf.Lerp(0.1f, 0.25f, GD.Randf()))
			// 	.Append(_offset.TweenScale(0f, Mathf.Lerp(0.1f, 0.2f, GD.Randf())).SetEasing(Easing.OutQuad))
			// 	.Build().Play();
		}, Mathf.Lerp(0.4f, 0.5f, GD.Randf()));
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if(_velocity != Vector2.Zero)
		{
			_velocity += Vector2.Down * (float)delta * 1000;
		}

		SetPosition(Position += (float)delta * _velocity);
		_offset.SetRotation(_offset.Rotation + (float)delta * _angularVelocity);
	}
}