using System;
using Godot;
using GTweens.Easings;

public partial class SelectFigureViewFigure : Control
{
	[Export]
	private BetterButton _button;
	[Export]
	private Control _scaleContainer;
	[Export]
	private Sprite2D _outline;
	[Export]
	private Sprite2D _sprite;

	public Figure Figure { get; private set; }

	public event Action<SelectFigureViewFigure> PressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_button.Pressed += OnPressed;
	}

	public void Init(Figure figure)
	{
		Figure = figure;

		_scaleContainer.SetScale(Vector2.Zero);
		_scaleContainer.SetPivotOffset(_scaleContainer.Size * 0.5f);
		_scaleContainer.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();

		_outline.SetSelfModulate(figure.OutlineColor);

		Texture2D mapIconTexture = figure.MapIconTexture;
		_sprite.SetTexture(mapIconTexture);
		float textureWidth = mapIconTexture.GetWidth();
		_sprite.SetScale((250f / textureWidth) * Vector2.One);
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}
}