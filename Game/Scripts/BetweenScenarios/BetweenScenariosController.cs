using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;

public partial class BetweenScenariosController : SceneController<BetweenScenariosController>
{
	[Export]
	public BetweenScenariosSidePanel BetweenScenariosSidePanel { get; private set; }

	[Export]
	public BetweenScenariosCharacterPortraitManager CharacterPortraitManager { get; private set; }

	[Export]
	public ScenarioFlowchart ScenarioFlowchart { get; private set; }

	[Export]
	public EventOverlay EventOverlay { get; private set; }

	[Export]
	public BetweenScenariosActionManager ActionManager { get; private set; }

	[Export]
	public ItemShop ItemShop { get; private set; }

	[Export]
	public CharacterCreationOverlay CharacterCreationOverlay { get; private set; }

	[Export]
	public BetweenScenariosClassUnlockOverlay UnlockOverlay { get; private set; }

	private readonly List<EventReward> _duringDowntimeEventRewards = new List<EventReward>();

	public BetweenScenariosSceneRequest SceneRequest { get; private set; }

	public RandomNumberGenerator RNG { get; private set; }

	public BetweenScenariosEvents Events { get; private set; }

	public SavedCampaign SavedCampaign => SceneRequest.SavedCampaign;

	public override void _EnterTree()
	{
		base._EnterTree();

		SceneRequest = AppController.Instance.SceneLoader.CurrentSceneRequest as BetweenScenariosSceneRequest;

		if(SceneRequest == null)
		{
			SceneRequest = new BetweenScenariosSceneRequest(SavedCampaign.Test());
		}

		RNG = new RandomNumberGenerator();
		RNG.Randomize();

		Events = new BetweenScenariosEvents();

		AppController.Instance.AudioController.SetBGM("res://Audio/BGM/old-tavern-cinematic-atmosphere-fairytale-273871.mp3");
		AppController.Instance.AudioController.SetBGS(null);
	}

	public override void _Ready()
	{
		base._Ready();

		StartSequence().Forget();
	}

	public override void _ExitTree()
	{
		for(int i = _duringDowntimeEventRewards.Count - 1; i >= 0; i--)
		{
			UnsubscribeDuringDowntime(_duringDowntimeEventRewards[i]);
		}

		if(SavedCampaign != null)
		{
			foreach(SavedCharacter savedCharacter in SavedCampaign.Characters)
			{
				savedCharacter.SavedPersonalQuest.Model.OnBetweenScenariosEnded(savedCharacter);
			}
		}

		base._ExitTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
		{
			if(inputEventKey.Keycode == Key.Escape)
			{
				OpenMenuPopup();
			}

			if(OS.IsDebugBuild())
			{
				if(inputEventKey.Keycode == Key.X)
				{
					foreach(SavedCharacter savedCharacter in SavedCampaign.Characters)
					{
						savedCharacter.AddXP(30);
					}
				}

				if(inputEventKey.Keycode == Key.P)
				{
					SavedCampaign.AdjustProsperity(1);
				}

				if(inputEventKey.Keycode == Key.R)
				{
					if(Input.IsKeyPressed(Key.Shift))
					{
						SavedCampaign.AdjustReputation(-1);
					}
					else
					{
						SavedCampaign.AdjustReputation(1);
					}
				}
			}
		}
	}

	public override void _Notification(int what)
	{
		base._Notification(what);

		if(what == NotificationWMGoBackRequest)
		{
			OpenMenuPopup();
		}
	}

	public void TryStartScenario(ScenarioModel scenarioModel)
	{
		if(SavedCampaign.Characters.Count < 2)
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Cannot start scenario",
				"You need at least 2 characters in order to start a scenario."));

			return;
		}

		if(SavedCampaign.Characters.Any(character => character.GetCanRetire(SavedCampaign)))
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Cannot start scenario",
				"One of your characters is ready to retire."));

			return;
		}

		if(SavedCampaign.Characters.Any(character => character.CheckCanLevelUp() || character.LevelUpInProgress))
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Cannot start scenario",
				"One of your characters is ready to level up."));

			return;
		}

		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request($"Scenario {scenarioModel.ScenarioNumber}",
			$"Start scenario {scenarioModel.ScenarioNumber}?",
			new TextButton.Parameters("Cancel",
				() =>
				{
				}
			),
			new TextButton.Parameters("Confirm",
				() =>
				{
					StartScenarioSequence(scenarioModel).Forget();
				},
				TextButton.ColorType.Green
			)
		));
	}

	public void RetireCharacter(SavedCharacter savedCharacter, SavedCampaign savedCampaign)
	{
		AppController.Instance.PopupManager.RequestPopup(new RetirementPopup.Request()
		{
			Character = savedCharacter,
			SavedCampaign = savedCampaign,
			UnlockedClass = savedCampaign.GetUnlockedClass(savedCharacter)
		});

		savedCampaign.RetireCharacter(savedCharacter);
	}

	public void UnsubscribeDuringDowntime(EventReward eventReward)
	{
		eventReward.UnsubscribeDuringDowntime();
		_duringDowntimeEventRewards.Remove(eventReward);
	}

	private async GDTaskVoid StartSequence()
	{
		CancellationToken cancellationToken = DestroyCancellationToken;

		await GDTask.Yield(cancellationToken);
		await GDTask.Delay(0.2f, cancellationToken: cancellationToken);

		foreach(SavedCharacter savedCharacter in SavedCampaign.Characters)
		{
			await savedCharacter.SavedPersonalQuest.Model.OnBetweenScenariosStarted(savedCharacter);
		}

		if(SavedCampaign.SavedEvents.CanDrawCityEvent && SavedCampaign.SavedEvents.CityEventDeckIds.Count > 0)
		{
			await EventOverlay.DrawEventCard(EventType.City, cancellationToken);
		}

		foreach(SavedEventState savedEventState in SavedCampaign.SavedEvents.SavedEventStates)
		{
			foreach(EventReward eventReward in savedEventState.Choice.GetRewards(savedEventState))
			{
				if(eventReward.Type == EventRewardType.DuringDowntime)
				{
					eventReward.SubscribeDuringDowntime(savedEventState);

					_duringDowntimeEventRewards.Add(eventReward);
				}
			}
		}

		if(SceneRequest.SavedCampaign.Characters.Count == 0)
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Welcome!",
				"Welcome to the very early access version of The Crimson Scales!\nPlease create a couple of characters to get started on this campaign. " +
				"You can do so using the button in the bottom-left corner."
			));
		}

		// if(
		// 	SceneRequest.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario010>()).Completed &&
		// 	SceneRequest.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario013>()).Completed &&
		// 	SceneRequest.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario014>()).Completed)
		// {
		// 	AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("End of Demo",
		// 		"Thank you for playing this demo of The Crimson Scales!\nHope you had fun!" +
		// 		"\nAny and all feedback is very welcome. Please do not hesitate to let us know your thoughts."
		// 	));
		// }

		ActionManager.Init();
	}

	private async GDTaskVoid StartScenarioSequence(ScenarioModel scenarioModel)
	{
		CancellationToken cancellationToken = DestroyCancellationToken;

		BetweenScenariosEvents.DrawRoadEvent.Parameters drawRoadEventParameters =
			BetweenScenariosEvents.DrawRoadEventEvent.Fire(
				new BetweenScenariosEvents.DrawRoadEvent.Parameters());

		if(drawRoadEventParameters.DrawEvent)
		{
			await EventOverlay.DrawEventCard(EventType.Road, cancellationToken);
		}

		SavedCampaign savedCampaign = SavedCampaign;
		float characterLevelSum = savedCampaign.Characters.Sum(character => character.Level);
		int scenarioLevel =
			Mathf.CeilToInt((characterLevelSum / savedCampaign.Characters.Count) / 2f) +
			AppController.Instance.SaveFile.SaveData.Options.Difficulty.Value;
		scenarioLevel = Mathf.Clamp(scenarioLevel, 0, 7);
		savedCampaign.SetSavedScenario(new SavedScenario()
		{
			Id = Guid.NewGuid(),
			AppVersion = AppController.Instance.SaveFile.SaveData.AppVersion,
			ScenarioModelId = scenarioModel.Id.ToString(),
			Seed = GD.RandRange(0, int.MaxValue),
			ScenarioLevel = scenarioLevel,
			IsOnline = false
		});

		AppController.Instance.SaveFile.SaveData.SavedCampaign = savedCampaign;
		AppController.Instance.SceneLoader.RequestSceneChange(new GameSceneRequest(savedCampaign));
	}

	private void OpenMenuPopup()
	{
		this.DelayedCall(() =>
		{
			if(!AppController.Instance.PopupManager.IsPopupOpen())
			{
				AppController.Instance.PopupManager.RequestPopup(new MenuPopup.Request());
			}
		});
	}
}