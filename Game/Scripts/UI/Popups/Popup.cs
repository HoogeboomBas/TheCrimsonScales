using System;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public abstract partial class Popup<T> : PopupBase
	where T : PopupRequest
{
	private Panel _background;
	private BetterButton _backgroundButton;
	protected PanelContainer _panelContainer;
	private BetterButton _closeButton;
	private Control _header;
	protected Label _title;

	private bool _canClose;

	public T PopupRequest { get; private set; }

	public bool IsOpened { get; private set; }

	public override Type RequestType => typeof(T);

	public override void _Ready()
	{
		base._Ready();

		_background = GetNode<Panel>("Background");
		_backgroundButton = GetNode<BetterButton>("Background/Button");
		_panelContainer = GetNode<PanelContainer>("Panel");
		_closeButton = GetNode<BetterButton>("Panel/MarginContainer/VBoxContainer/Header/CloseButton");
		_header = GetNode<Control>("Panel/MarginContainer/VBoxContainer/Header");
		_title = GetNode<Label>("Panel/MarginContainer/VBoxContainer/Header/Label");

		_canClose = true;

		_closeButton.Pressed += OnClosePressed;
		_backgroundButton.Pressed += OnBackgroundPressed;

		//CallDeferred(MethodName.Hide);
		SetVisible(false);
	}

	public sealed override void Open(PopupRequest request)
	{
		PopupRequest = (T)request;

		base.Open(request);

		OnOpen();

		GTweenSequenceBuilder sequenceBuilder = GTweenSequenceBuilder.New();
		OpenAnimation(sequenceBuilder);
		sequenceBuilder.AppendCallback(OnOpened);
		sequenceBuilder.Build().Play();
	}

	public sealed override void Close()
	{
		if(!_canClose)
		{
			return;
		}

		base.Close();

		OnClose();

		GTweenSequenceBuilder sequenceBuilder = GTweenSequenceBuilder.New();
		CloseAnimation(sequenceBuilder);
		sequenceBuilder.AppendCallback(OnClosed);
		sequenceBuilder.AppendCallback(OnClosedFinal);
		sequenceBuilder.Build().Play();
	}

	protected void SetCanClose(bool canClose)
	{
		_canClose = canClose;

		_closeButton.SetVisible(_canClose);
	}

	protected virtual void OnOpen()
	{
		SetVisible(true);

		FirePopupOpenEvent(PopupRequest);

		// Hack to fix layout
		_header.SetCustomMinimumSize(new Vector2(0f, 66f));
		this.DelayedCall(() =>
		{
			_header.SetCustomMinimumSize(new Vector2(0f, 65f));
		});
	}

	protected virtual void OnOpened()
	{
		IsOpened = true;
	}

	protected virtual void OpenAnimation(GTweenSequenceBuilder sequenceBuilder)
	{
		_background.SelfModulate = Colors.Transparent;
		_panelContainer.Scale = 0.001f * Vector2.One;

		this.DelayedCall(() =>
		{
			_panelContainer.PivotOffset = _panelContainer.Size * 0.5f;
		});

		sequenceBuilder.Append(_background.TweenSelfModulateAlpha(1f, 0.2f));
		sequenceBuilder.Join(_panelContainer.TweenScale(1f, 0.2f).SetEasing(Easing.OutBack));
	}

	protected virtual void OnClose()
	{
		IsOpened = false;
	}

	protected virtual void OnClosed()
	{
		SetVisible(false);
	}

	private void OnClosedFinal()
	{
		FirePopupClosedEvent(PopupRequest);
	}

	protected virtual void CloseAnimation(GTweenSequenceBuilder sequenceBuilder)
	{
		_panelContainer.Scale = Vector2.One;
		_panelContainer.PivotOffset = _panelContainer.Size * 0.5f;

		sequenceBuilder.Append(_background.TweenSelfModulateAlpha(0f, 0.2f));
		sequenceBuilder.Join(_panelContainer.TweenScale(0.001f, 0.2f).SetEasing(Easing.InBack));
	}

	private void OnClosePressed()
	{
		Close();
	}

	private void OnBackgroundPressed()
	{
		Close();
	}
}