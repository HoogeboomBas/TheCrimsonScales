using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PopupManager : Node
{
	private readonly Dictionary<Type, PopupBase> _popups = new Dictionary<Type, PopupBase>();

	private readonly List<PopupRequest> _requests = new List<PopupRequest>();
	private readonly List<PopupRequest> _openRequests = new List<PopupRequest>();

	public event Action<PopupRequest> PopupOpenEvent;
	public event Action<PopupRequest> PopupClosedEvent;

	public override void _Ready()
	{
		base._Ready();

		List<PopupBase> popups = this.GetChildrenOfType<PopupBase>();
		foreach(PopupBase popup in popups)
		{
			popup.PopupClosedEvent += OnPopupClosed;
			popup.PopupOpenEvent += OnPopupOpen;
			_popups.Add(popup.RequestType, popup);
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
		{
			if(inputEventKey.Keycode == Key.Escape)
			{
				CloseTopPopup();
			}
		}
	}

	public override void _Notification(int what)
	{
		base._Notification(what);

		if(what == NotificationWMGoBackRequest)
		{
			CloseTopPopup();
		}
	}

	public void RequestPopup(PopupRequest request)
	{
		_requests.Add(request);

		TryOpenFirstPopup();
	}

	public void OpenPopupOnTop<T>(T request)
		where T : PopupRequest
	{
		if(IsPopupOpen<T>())
		{
			Log.Error($"Popup with request type {request.GetType()} already opened");
			return;
		}

		OpenPopup(request);
	}

	public void TryOpenPopupOnTop<T>(T request)
		where T : PopupRequest
	{
		if(IsPopupOpen<T>())
		{
			return;
		}

		OpenPopupOnTop(request);
	}

	public void CloseTopPopup()
	{
		if(_openRequests.Count > 0)
		{
			PopupBase popup = GetPopup(_openRequests[_openRequests.Count - 1]);
			popup.Close();
		}
	}

	public void CloseAll()
	{
		foreach(PopupRequest openRequest in _openRequests)
		{
			PopupBase popup = GetPopup(openRequest);
			popup.Close();
		}

		//_openRequests.Clear();

		_requests.Clear();
	}

	public bool IsPopupOpen<T>()
		where T : PopupRequest
	{
		foreach(PopupRequest openRequest in _openRequests)
		{
			if(openRequest is T)
			{
				return true;
			}
		}

		return false;
	}

	public bool IsPopupOpen()
	{
		return _openRequests.Count > 0 || _requests.Count > 0;
	}

	private void TryOpenFirstPopup()
	{
		if(_openRequests.Count > 0 || _requests.Count == 0)
		{
			return;
		}

		PopupRequest request = _requests[0];
		_requests.RemoveAt(0);
		OpenPopup(request);
	}

	private void OpenPopup(PopupRequest request)
	{
		PopupBase popup = GetPopup(request);
		popup.Open(request);
		MoveChild(popup, _popups.Count - 1);
		_openRequests.Add(request);
	}

	private PopupBase GetPopup(PopupRequest request)
	{
		return GetPopup(request.GetType());
	}

	private PopupBase GetPopup(Type requestType)
	{
		return _popups[requestType];
	}

	private void OnPopupOpen(PopupRequest request)
	{
		PopupOpenEvent?.Invoke(request);
	}

	private void OnPopupClosed(PopupRequest request)
	{
		if(_openRequests.Contains(request))
		{
			_openRequests.Remove(request);
		}

		TryOpenFirstPopup();

		PopupClosedEvent?.Invoke(request);
	}
}