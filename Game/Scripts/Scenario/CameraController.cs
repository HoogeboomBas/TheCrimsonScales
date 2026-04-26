using Godot;

public partial class CameraController : Node
{
	[Export]
	public Camera2D Camera { get; private set; }

	[Export]
	private Node2D _movePivot;
	[Export]
	private float _moveSpeed;
	[Export]
	private float _moveLerpSpeed;
	[Export]
	private float _zoomSpeed;
	[Export]
	private float _zoomLerpSpeed;
	[Export]
	private float _minZoom;
	[Export]
	private float _maxZoom;
	[Export]
	private StringName _moveRightAction;
	[Export]
	private StringName _moveLeftAction;
	[Export]
	private StringName _moveUpAction;
	[Export]
	private StringName _moveDownAction;

	private static Vector2 _moveTarget;
	private static Vector2 _zoomTarget;

	public override void _Ready()
	{
		if(GameController.Instance.SceneRequest.FromUndo)
		{
			_movePivot.Position = _moveTarget;
			Camera.Zoom = _zoomTarget;
		}
		else
		{
			_moveTarget = Vector2.Zero;
			_zoomTarget = Camera.Zoom;
		}

		AppController.Instance.InputController.DragEvent += OnDrag;
		AppController.Instance.InputController.MiddleMouseDragEvent += OnDrag;
		AppController.Instance.InputController.MagnifyEvent += OnMagnify;
	}

	public override void _ExitTree()
	{
		AppController.Instance.InputController.DragEvent -= OnDrag;
		AppController.Instance.InputController.MiddleMouseDragEvent -= OnDrag;
		AppController.Instance.InputController.MagnifyEvent -= OnMagnify;
	}

	public override void _Process(double delta)
	{
		Vector2 moveInput = Vector2.Zero;

		if(Input.IsActionPressed(_moveRightAction))
		{
			moveInput.X = 1f;
		}

		if(Input.IsActionPressed(_moveLeftAction))
		{
			moveInput.X = -1f;
		}

		if(Input.IsActionPressed(_moveUpAction))
		{
			moveInput.Y = -1f;
		}

		if(Input.IsActionPressed(_moveDownAction))
		{
			moveInput.Y = 1f;
		}

		Rect2 containerRect = GameController.Instance.Map.ContainerRect;
		_moveTarget += (float)delta * _moveSpeed * moveInput / _zoomTarget;
		_moveTarget.X = Mathf.Clamp(_moveTarget.X, containerRect.Position.X, containerRect.End.X);
		_moveTarget.Y = Mathf.Clamp(_moveTarget.Y, containerRect.Position.Y, containerRect.End.Y);

		_movePivot.Position = _movePivot.Position.Lerp(_moveTarget, (float)delta * 60f * _moveLerpSpeed);

		_zoomTarget = _zoomTarget.Clamp(_minZoom * Vector2.One, _maxZoom * Vector2.One);
		Camera.Zoom = Camera.Zoom.Lerp(_zoomTarget, (float)delta * 60f * _zoomLerpSpeed);

		if(moveInput != Vector2.Zero || (_zoomTarget.DistanceTo(Camera.Zoom) > 0.001f))
		{
			AppController.Instance.InputController.UpdateDrag();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(GameController.Instance.CursorOverUIChecker.CursorOverUI)
		{
			return;
		}

		if(@event is InputEventMouseButton button && button.IsPressed()) // button.ButtonMask == MouseButtonMask.Middle)
		{
			if(button.ButtonIndex == MouseButton.WheelUp)
			{
				_zoomTarget *= 1 + _zoomSpeed;
			}

			if(button.ButtonIndex == MouseButton.WheelDown)
			{
				_zoomTarget *= 1 - _zoomSpeed;
			}
		}

		if(Platform.DeskTop)
		{
			if(@event is InputEventPanGesture panGesture)
			{
				_moveTarget += 50f * panGesture.Delta / _zoomTarget;
			}
		}
	}

	private void OnDrag(Vector2 previousPosition, Vector2 currentPosition)
	{
		Vector2 delta = currentPosition - previousPosition;
		_moveTarget -= delta / _zoomTarget;
	}

	private void OnMagnify(float factor)
	{
		_zoomTarget *= factor;
	}
}