using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class Tuckbox : Node3D
{
	private static readonly Vector3 OffScreenPosition = new Vector3(0f, -10f, 0f);

	[Export]
	private Node3D _top;
	[Export]
	private Node3D _flop;
	[Export]
	private Node3D _floplip;

	private Vector3 _initialPosition;
	private Vector3 _initialFloplipRotation;

	public override void _Ready()
	{
		base._Ready();

		SetVisible(false);
		_initialPosition = Position;
		_initialFloplipRotation = _floplip.RotationDegrees;
	}

	public async GDTask AnimateIn(CancellationToken cancellationToken)
	{
		SetVisible(true);
		SetPosition(_initialPosition + OffScreenPosition);
		SetRotationDegrees(new Vector3(0f, 180f, 0f));
		_top.SetRotationDegrees(new Vector3(-90f, 0f, 0f));
		_flop.SetRotationDegrees(new Vector3(-90f, 0f, 0f));
		_floplip.SetRotationDegrees(_initialFloplipRotation);

		await this.TweenPosition(_initialPosition, 1f).SetEasing(Easing.OutBack).PlayAsync(cancellationToken);

		await GDTask.Delay(0.5f, cancellationToken: cancellationToken);

		await this.TweenRotationY(0f, 0.8f).SetEasing(Easing.OutBack).PlayAsync(cancellationToken);
	}

	public async GDTask OpenAnimation(CancellationToken cancellationToken)
	{
		this.DelayedCall(() => this.TweenPosition(_initialPosition + OffScreenPosition * 2, 1f).SetEasing(Easing.InSine).Play(), 0.2f);

		_top.TweenRotationX(0f, 0.6f).SetEasing(Easing.OutBack).Play();
		await GDTask.Delay(0.2f, cancellationToken: cancellationToken);
		_flop.TweenRotationX(0f, 0.6f).SetEasing(Easing.OutBack).Play();
		await GDTask.Delay(0.1f, cancellationToken: cancellationToken);
		_floplip.TweenRotationX(0f, 0.3f).SetEasing(Easing.OutBack).Play();
		//await GDTask.Delay(0.1f, cancellationToken: cancellationToken);

		await GDTask.Delay(1f, cancellationToken: cancellationToken);
		SetVisible(false);
	}
}