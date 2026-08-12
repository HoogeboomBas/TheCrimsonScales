using System.Collections.Generic;
using Godot;
using GTweens.Easings;

public partial class InfoView : Control
{
	[Export]
	private Control _container;
	[Export]
	private Control _pinButtonContainer;
	[Export]
	private BetterButton _pinButton;

	[Export]
	private PackedScene _panelScene;
	[Export]
	private Control _panelContainer;

	private Hex _hoveredHex;
	private Hex _pinnedHex;
	private Hex _visibleHex;

	private InfoViewPanel _panel;

	public override void _Ready()
	{
		base._Ready();

		_pinButtonContainer.Scale = Vector2.Zero;

		AppController.Instance.InputController.SecondaryButtonPressedEvent += OnSecondaryButtonPressed;

		GameController.Instance.Selector.CoordsChangedEvent += OnSelectorCoordsChanged;

		_pinButton.Pressed += OnPinButtonPressed;
		GameController.Instance.HexPin.PressedEvent += OnPinButtonPressed;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		AppController.Instance.InputController.SecondaryButtonPressedEvent -= OnSecondaryButtonPressed;
	}

	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	if(@event is InputEventMouseButton mouseButton)
	// 	{
	// 		if(mouseButton.ButtonIndex == MouseButton.Right && !@event.IsPressed())
	// 		{
	// 			SetPinned(_hoveredHex);
	// 		}
	// 	}
	// }

	private void UpdateView(Hex hex)
	{
		if(_visibleHex != null)
		{
			_visibleHex.HexObjectsChangedEvent -= OnHexObjectsChanged;
		}

		_visibleHex = hex;

		if(_visibleHex != null)
		{
			_visibleHex.HexObjectsChangedEvent += OnHexObjectsChanged;
		}

		_panel?.Destroy();
		_panel = null;

		if(_visibleHex == null)
		{
		}
		else
		{
			List<InfoItemParameters> parametersList = new List<InfoItemParameters>();

			foreach(Marker marker in GameController.Instance.Map.Markers)
			{
				if(marker.Visible && marker.Hex == _visibleHex)
				{
					parametersList.Add(new MarkerInfoItem.Parameters(marker));
				}
			}

			foreach(HexObject hexObject in _visibleHex.HexObjects)
			{
				if(hexObject.Visible)
				{
					hexObject.AddInfoItemParameters(parametersList);
				}
			}

			if(parametersList.Count > 0)
			{
				_panel = _panelScene.Instantiate<InfoViewPanel>();
				_panelContainer.AddChild(_panel);
				_panel.Init(parametersList, _container.Size.Y);
				_panel!.SetCanClick(false);

				GameController.Instance.HexPin.SetHex(hex);
			}
		}
	}

	private void SetPinned(Hex hex)
	{
		bool switchingPin = false;

		if(_pinnedHex == hex)
		{
			_pinnedHex = null;
		}
		else
		{
			switchingPin = _pinnedHex != null && hex != null;

			_pinnedHex = hex;
		}

		UpdateView(_hoveredHex);

		if(_panel == null && _pinnedHex != null)
		{
			_pinnedHex = null;
			UpdateView(null);
			_pinButtonContainer.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).PlayFastForwardable();
			GameController.Instance.HexPin.SetActive(false);
		}
		else if(switchingPin)
		{
			_pinButtonContainer.TweenPulse(1.4f, 0.15f).SetEasing(Easing.OutBack).PlayFastForwardable();
			_panel!.SetCanClick(true);
			GameController.Instance.HexPin.PulsePin();
		}
		else if(_pinnedHex != null)
		{
			_pinButtonContainer.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).PlayFastForwardable();
			_panel!.SetCanClick(true);
			GameController.Instance.HexPin.SetActive(true);
		}
		else
		{
			_pinButtonContainer.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).PlayFastForwardable();
			GameController.Instance.HexPin.SetActive(false);
		}
	}

	private void OnSecondaryButtonPressed()
	{
		SetPinned(_hoveredHex);
	}

	private void OnSelectorCoordsChanged(Vector2I coords, bool visible)
	{
		_hoveredHex = visible ? GameController.Instance.Map.GetHex(coords) : null;

		if(_pinnedHex == null)
		{
			UpdateView(_hoveredHex);
		}
	}

	private void OnHexObjectsChanged(Hex hex)
	{
		if(hex != _visibleHex)
		{
			Log.Error("Subscribed to the wrong hex events.");
			return;
		}

		this.DelayedCall(() =>
		{
			if(hex == _visibleHex)
			{
				UpdateView(_visibleHex);
				_panel?.SetCanClick(_pinnedHex != null);

				if(_panel == null && _pinnedHex != null)
				{
					SetPinned(null);
				}
			}
		});
	}

	private void OnPinButtonPressed()
	{
		SetPinned(null);
	}
}