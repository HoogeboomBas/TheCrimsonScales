using System.Collections.Generic;
using Godot;
using GTweensGodot.Extensions;

public partial class ScenarioButton : Control
{
	private Timer _titleHoverTimer;
	private PopupPanel _titleTooltip;

	[Export]
	public int ScenarioNumber { get; private set; }

	[Export]
	private BetterButton _betterButton;
	[Export]
	private Label _label;

	[Export]
	private Control _completedContainer;

	[Export]
	private PackedScene _scenarioButtonOutlineScene;

	[Export]
	private PackedScene _scenarioFlowchartArrowScene;

	public Vector2I Coords { get; private set; }
	public List<ScenarioFlowchartArrow> Arrows { get; } = new List<ScenarioFlowchartArrow>();
	public SavedScenarioProgress SavedScenarioProgress { get; private set; }
	public ScenarioButtonOutline ScenarioButtonOutline { get; private set; }

	public string ModelId => $"SCENARIO_MODEL.SCENARIO{ScenarioNumber:D3}";
	public ScenarioModel Model => ModelDB.GetById<ScenarioModel>(ModelId);

	public override void _Ready()
	{
		base._Ready();

		if(Model == null)
		{
			_betterButton.SetVisible(false);
			return;
		}

		_betterButton.Pressed += OnButtonPressed;
		InitTitleTooltip();
		_label.Text = Model.ScenarioNumber.ToString();
	}

	private void InitTitleTooltip()
	{
		_titleTooltip = new PopupPanel { ThemeTypeVariation = "TooltipPanel" };
		_titleTooltip.SetFlag(Window.Flags.NoFocus, true);
		_titleTooltip.SetFlag(Window.Flags.Popup, false);
		_titleTooltip.SetFlag(Window.Flags.MousePassthrough, true);
		_titleTooltip.AddChild(new Label { Text = Model.Name, ThemeTypeVariation = "TooltipLabel" });
		_betterButton.AddChild(_titleTooltip);

		_titleHoverTimer = new Timer { WaitTime = 0.1, OneShot = true };
		AddChild(_titleHoverTimer);
		_titleHoverTimer.Timeout += ShowTitleTooltip;
		_betterButton.MouseEntered += () => _titleHoverTimer.Start();
		_betterButton.MouseExited += HideTitleTooltip;
		_betterButton.ButtonDown += HideTitleTooltip;
		_betterButton.VisibilityChanged += HideTitleTooltip;
	}

	private void ShowTitleTooltip()
	{
		if(!_betterButton.IsVisibleInTree())
		{
			return;
		}

		Viewport viewport = GetViewport();
		Vector2I size = (Vector2I)_titleTooltip.GetContentsMinimumSize().Ceil();
		Vector2 position = viewport.GetMousePosition() + new Vector2(16, 16);
		position = position.Clamp(Vector2.Zero, (viewport.GetVisibleRect().Size - size).Max(Vector2.Zero));
		_titleTooltip.Popup(new Rect2I((Vector2I)position, size));
	}

	private void HideTitleTooltip()
	{
		_titleHoverTimer.Stop();
		_titleTooltip.Hide();
	}

	public void Init(Vector2I coords)
	{
		if(Model == null)
		{
			_betterButton.SetVisible(false);
			return;
		}

		Coords = coords;

		SavedCampaign savedCampaign = BetweenScenariosController.Instance.SavedCampaign;
		SavedScenarioProgress = savedCampaign.SavedScenarioProgresses.GetScenarioProgress(Model);

		ScenarioButtonOutline = _scenarioButtonOutlineScene.Instantiate<ScenarioButtonOutline>();
		BetweenScenariosController.Instance.ScenarioFlowchart.OutlineParent.AddChild(ScenarioButtonOutline);
		ScenarioButtonOutline.Init(this);

		foreach(ScenarioConnection connection in Model.Connections)
		{
			ScenarioFlowchartArrow arrow = _scenarioFlowchartArrowScene.Instantiate<ScenarioFlowchartArrow>();
			AddChild(arrow);
			ScenarioButton destinationScenarioButton =
				BetweenScenariosController.Instance.ScenarioFlowchart.GetScenarioButton(connection.To.ScenarioNumber);
			arrow.Init(this, destinationScenarioButton, connection.Linked);
			Arrows.Add(arrow);
		}

		if(!SavedScenarioProgress.ShownOnMap)
		{
			ScenarioButtonOutline.SetVisible(false);
			_betterButton.SetVisible(false);
		}

		if(!SavedScenarioProgress.Completed)
		{
			_completedContainer.SetVisible(false);

			foreach(ScenarioFlowchartArrow arrow in Arrows)
			{
				arrow.SetVisible(false);
			}
		}
	}

	public void AnimateIn()
	{
		_betterButton.SetVisible(true);
		Scale = Vector2.Zero;

		this.TweenPulse(1.4f, 0.6f).Play();

		// GTweenSequenceBuilder.New()
		// 	.Append(this.TweenScale(1.2f, 0.5f))
		// 	.Append(this.TweenScale(1f, 0.5f))
		// 	// .Append(_container.TweenSizeX(sizeX, 0.5f))
		// 	// .Append(_linkContainer.TweenScale(Linked ? 1f : 0f, Linked ? 0.3f : 0f).SetEasing(Easing.OutBack))
		// 	.Build().Play();
	}

	private void OnButtonPressed()
	{
		BetweenScenariosController.Instance.TryStartScenario(Model);
	}
}