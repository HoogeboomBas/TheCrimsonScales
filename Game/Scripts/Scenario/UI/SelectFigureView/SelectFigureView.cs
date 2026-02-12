using System;
using System.Collections.Generic;
using Godot;

public partial class SelectFigureView : Control
{
	[Export]
	private Control _figureParent;
	[Export]
	private PackedScene _selectFigureViewFigureScene;
	[Export]
	private BetterButton _backgroundButton;

	private readonly List<SelectFigureViewFigure> _figures = new List<SelectFigureViewFigure>();

	private event Action<Figure> FigurePressedEvent;

	public override void _Ready()
	{
		base._Ready();

		SetVisible(false);

		_backgroundButton.Pressed += OnBackgroundPressed;
	}

	public void Open(List<Figure> figures, Action<Figure> onFigurePressed)
	{
		Close();

		SetVisible(true);

		FigurePressedEvent = onFigurePressed;

		this.DelayedCall(() =>
		{
			foreach(Figure figure in figures)
			{
				SelectFigureViewFigure selectFigureViewFigure = _selectFigureViewFigureScene.Instantiate<SelectFigureViewFigure>();
				_figureParent.AddChild(selectFigureViewFigure);
				selectFigureViewFigure.Init(figure);
				selectFigureViewFigure.PressedEvent += OnFigurePressed;
				_figures.Add(selectFigureViewFigure);
			}
		});
	}

	public void Close()
	{
		foreach(SelectFigureViewFigure figure in _figures)
		{
			figure.QueueFree();
		}

		_figures.Clear();

		SetVisible(false);
	}

	private void OnFigurePressed(SelectFigureViewFigure figure)
	{
		FigurePressedEvent?.Invoke(figure.Figure);
	}

	private void OnBackgroundPressed()
	{
		Close();
	}
}