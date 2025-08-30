using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GTweensGodot.Extensions;

public partial class ScenarioButton : Control
{
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
		_label.Text = Model.ScenarioNumber.ToString();
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
			ScenarioButton destinationScenarioButton = BetweenScenariosController.Instance.ScenarioFlowchart.GetScenarioButton(connection.To.ScenarioNumber);
			arrow.Init(this, destinationScenarioButton, connection.Linked);
			Arrows.Add(arrow);
		}

		if(!SavedScenarioProgress.Discovered)
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
		if(BetweenScenariosController.Instance.SavedCampaign.Characters.Count < 2)
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Cannot start scenario",
				"You need at least 2 characters in order to start a scenario.\n"));

			return;
		}

		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request($"Scenario {ScenarioNumber}", $"Start scenario {ScenarioNumber}?",
			new TextButton.Parameters("Cancel",
				() =>
				{
				}
			),
			new TextButton.Parameters("Confirm",
				() =>
				{
					SavedCampaign savedCampaign = BetweenScenariosController.Instance.SavedCampaign;
					float characterLevelSum = savedCampaign.Characters.Sum(character => character.Level);
					int scenarioLevel = Mathf.CeilToInt((characterLevelSum / savedCampaign.Characters.Count) / 2f) + AppController.Instance.SaveFile.SaveData.Options.Difficulty.Value;
					scenarioLevel = Mathf.Clamp(scenarioLevel, 0, 7);
					savedCampaign.SavedScenario = new SavedScenario()
					{
						Id = Guid.NewGuid(),
						AppVersion = AppController.Instance.SaveFile.SaveData.AppVersion,
						ScenarioModelId = ModelId,
						Seed = GD.RandRange(0, int.MaxValue),
						ScenarioLevel = scenarioLevel,
						IsOnline = false
					};

					AppController.Instance.SaveFile.SaveData.SavedCampaign = savedCampaign;
					AppController.Instance.SceneLoader.RequestSceneChange(new GameSceneRequest(savedCampaign));
				},
				TextButton.ColorType.Green
			)
		));
	}
}