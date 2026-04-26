using System;
using System.Collections.Generic;
using Godot;

public partial class InputController : Node
{
	private class Touch
	{
		public int Index { get; init; }
		public Vector2 CurrentPos { get; set; }
	}

	public const float LongPressDuration = 0.5f;

	[Export]
	private float _dragThreshold;
	[Export]
	private StringName _skipAIDecisionAction;

	private readonly List<Touch> _touches = new List<Touch>();

	private float _dragDistance;
	private bool _releasedTouches;
	private Vector2 _previousDragPosition;

	private int _maxTouchCountInChain;

	public bool Pressing { get; private set; }
	public bool Dragging { get; private set; }

	public float PressDuration { get; private set; }

	//public bool Magnifying { get; private set; }
	public WorldButton DraggingObject { get; private set; }

	//public bool CanClick => !Dragging && !Magnifying && !AppController.Instance.SceneLoader.IsTransitioning;
	public bool CanClick => !Dragging && !AppController.Instance.SceneLoader.IsTransitioning;

	public bool IsDraggingObject => DraggingObject != null && DraggingObject.IsInsideTree();

	public bool LongPressing => PressDuration > LongPressDuration && !Dragging && _maxTouchCountInChain == 1;

	public event Action<float> PressDurationChangedEvent;

	public event Action<Vector2> DragStartEvent;
	public event Action<Vector2, Vector2> DragEvent;
	public event Action<Vector2> DragEndEvent;

	public event Action<Vector2> MiddleMouseDragStartEvent;
	public event Action<Vector2, Vector2> MiddleMouseDragEvent;
	public event Action<Vector2> MiddleMouseDragEndEvent;

	public event Action<float> MagnifyEvent;
	public event Action<InputEvent> InputEvent;
	public event Action<InputEvent> UnhandledInputEvent;
	public event Action ReleasedAllTouchesEvent;

	public event Action SecondaryButtonPressedEvent;

	public event Action SkipAIDecisionEvent;

	public override void _Input(InputEvent @event)
	{
		InputEvent?.Invoke(@event);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventScreenTouch screenTouch)
		{
			if(screenTouch.Pressed)
			{
				_touches.Add(new Touch()
				{
					Index = screenTouch.Index,
					CurrentPos = screenTouch.Position
				});

				_maxTouchCountInChain = Mathf.Max(_maxTouchCountInChain, _touches.Count);

				Pressing = true;
			}
			else
			{
				int touchIndex = _touches.FindIndex(touch => touch.Index == screenTouch.Index);
				if(touchIndex >= 0)
				{
					_touches.RemoveAt(touchIndex);
				}

				if(_touches.Count == 0)
				{
					_releasedTouches = true;

					// if(LongPressing)
					// {
					// 	SecondaryButtonPressedEvent?.Invoke();
					// }
				}
			}
		}

		if(@event is InputEventScreenDrag screenDrag) // && screenDrag.Index == 0)
		{
			float prevMagnifyDistance = 0f;
			if(_touches.Count == 2)
			{
				prevMagnifyDistance = _touches[0].CurrentPos.DistanceTo(_touches[1].CurrentPos);
			}

			Touch touch = _touches.Find(touch => touch.Index == screenDrag.Index);
			if(touch != null)
			{
				touch.CurrentPos = screenDrag.Position;
			}

			switch(_touches.Count)
			{
				case 1:
					Drag(screenDrag.Position - screenDrag.Relative, screenDrag.Position);
					break;
				case 2:
					float newMagnifyDistance = _touches[0].CurrentPos.DistanceTo(_touches[1].CurrentPos);
					float magnifyFactor = newMagnifyDistance / prevMagnifyDistance;
					MagnifyEvent?.Invoke(magnifyFactor);
					break;
			}
		}

		if(@event is InputEventMouseButton mouseButton)
		{
			if(mouseButton.ButtonIndex == MouseButton.Middle)
			{
				if(mouseButton.Pressed)
				{
					MiddleMouseDragStartEvent?.Invoke(mouseButton.Position);
				}
				else
				{
					MiddleMouseDragEndEvent?.Invoke(mouseButton.Position);
				}
			}

			if(mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed)
			{
				SecondaryButtonPressedEvent?.Invoke();
			}
		}

		if(@event is InputEventMouseMotion mouseMotion)
		{
			if((mouseMotion.ButtonMask & MouseButtonMask.Middle) != 0)
			{
				MiddleMouseDragEvent?.Invoke(mouseMotion.Position - mouseMotion.Relative, mouseMotion.Position);
			}
		}

		// if(@event is InputEventScreenDrag screenDrag && screenDrag.Index == 0)
		// {
		// 	Drag(screenDrag.Position - screenDrag.Relative, screenDrag.Position);
		// }

		if(Platform.DeskTop)
		{
			if(@event is InputEventMagnifyGesture magnifyGesture)
			{
				// Magnifying = true;
				MagnifyEvent?.Invoke(magnifyGesture.Factor);
			}
		}

		if(@event.IsActionPressed(_skipAIDecisionAction, true))
		{
			SkipAIDecisionEvent?.Invoke();
		}

		UnhandledInputEvent?.Invoke(@event);
	}

	public override void _Process(double delta)
	{
		if(_releasedTouches)
		{
			_maxTouchCountInChain = 0;

			Pressing = false;
			PressDuration = 0f;
			PressDurationChangedEvent?.Invoke(PressDuration);

			if(Dragging)
			{
				ResetDrag();
			}

			DraggingObject = null;

			//Magnifying = false;

			ReleasedAllTouchesEvent?.Invoke();

			_releasedTouches = false;
		}

		if(Pressing)
		{
			bool prevLongPressing = LongPressing;

			PressDuration += (float)delta;
			PressDurationChangedEvent?.Invoke(PressDuration);

			if(LongPressing && !prevLongPressing && !Platform.DeskTop)
			{
				SecondaryButtonPressedEvent?.Invoke();
			}
		}
	}

	public void SetDraggingObject(WorldButton node)
	{
		if(IsDraggingObject && DraggingObject != node)
		{
			Log.Error("Trying to drag a new object while an old object is being dragged.");
			return;
		}

		DraggingObject = node;
	}

	public void ResetDraggingObject(WorldButton node)
	{
		if(DraggingObject != node)
		{
			Log.Error("Trying to stop dragging an object which is not the currently actively dragged object.");
			return;
		}

		DraggingObject = null;
	}

	public void UpdateDrag()
	{
		if(IsDraggingObject)
		{
			Drag(_previousDragPosition, _previousDragPosition);
		}
	}

	private void Drag(Vector2 previousPosition, Vector2 currentPosition)
	{
		// if(Magnifying)
		// {
		// 	return;
		// }

		_dragDistance += (previousPosition - currentPosition).Length();

		if(_dragDistance >= _dragThreshold)
		{
			if(!Dragging && !IsDraggingObject)
			{
				DragStartEvent?.Invoke(currentPosition);
			}

			Dragging = true;

			if(!IsDraggingObject)
			{
				DragEvent?.Invoke(previousPosition, currentPosition);
			}
			else
			{
				DraggingObject.OnDrag(previousPosition, currentPosition);
			}
		}

		_previousDragPosition = currentPosition;
	}

	private void ResetDrag()
	{
		_dragDistance = 0f;
		Dragging = false;

		if(!IsDraggingObject)
		{
			DragEndEvent?.Invoke(_previousDragPosition);
		}
		else
		{
			DraggingObject.OnDragEnd(_previousDragPosition);
		}
	}
}