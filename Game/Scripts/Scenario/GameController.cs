using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

public partial class GameController : SceneController<GameController>
{
	private static string DefaultSavedGame;
	private static bool ShownIntroduction;

	[Export]
	public CameraController CameraController { get; private set; }

	[Export]
	public CursorOverUIChecker CursorOverUIChecker { get; private set; }

	[Export]
	public CardSelectionView CardSelectionView { get; private set; }

	[Export]
	public ShortRestView ShortRestView { get; private set; }

	[Export]
	public TreasureItemRewardView TreasureItemRewardView { get; private set; }

	[Export]
	public CardPlayView CardPlayView { get; private set; }

	[Export]
	public SelectFigureView SelectFigureView { get; private set; }

	[Export]
	public ChoiceButtonsView ChoiceButtonsView { get; private set; }

	[Export]
	public CardSelectionButtonsView CardSelectionButtonsView { get; private set; }

	[Export]
	public ScenarioSetupButtonsView ScenarioSetupButtonsView { get; private set; }

	[Export]
	public UndoView UndoView { get; private set; }

	[Export]
	public MovePath MovePath { get; private set; }

	[Export]
	public TeleportPath TeleportPath { get; private set; }

	[Export]
	public AOEView AOEView { get; private set; }

	[Export]
	public AOEButtonView AOEButtonView { get; private set; }

	[Export]
	public SufferDamageView SufferDamageView { get; private set; }

	[Export]
	public PortraitView PortraitView { get; private set; }

	[Export]
	public EffectSelectionView EffectSelectionView { get; private set; }

	[Export]
	public EffectInfoViewManager EffectInfoViewManager { get; private set; }

	[Export]
	public AMDDrawView AMDDrawView { get; private set; }

	[Export]
	public HintTextView HintTextView { get; private set; }

	[Export]
	public ElementsView ElementsView { get; private set; }

	[Export]
	public ScenarioLostView ScenarioLostView { get; private set; }

	[Export]
	public ScenarioWonView ScenarioWonView { get; private set; }

	[Export]
	public Selector Selector { get; private set; }

	[Export]
	public CharacterStartHexMoveIndicator CharacterStartHexMoveIndicator { get; private set; }

	[Export]
	public HexPin HexPin { get; private set; }

	[Export]
	public ScreenDistortion ScreenDistortion { get; private set; }

	[Export]
	public Node2D MoveParent { get; private set; }

	// Dirty, should be instantiated by the Hollowpact, but I cba right now :)
	[Export]
	public VoidSightView VoidSightView { get; private set; }

	private readonly Stopwatch _fastForwardStopwatch = new Stopwatch();

	public GameSceneRequest SceneRequest { get; private set; }

	public SavedCampaign SavedCampaign { get; private set; }

	public ReferenceManager ReferenceManager { get; private set; }
	public CardManager CardManager { get; private set; }

	public RandomNumberGenerator StateRNG { get; private set; }
	public RandomNumberGenerator VisualRNG { get; private set; }

	public PromptManager PromptManager { get; private set; }
	public SyncedActionManager SyncedActionManager { get; private set; }

	public Scenario Scenario { get; private set; }
	public ScenarioModel ScenarioModel { get; private set; }

	public ScenarioEvents ScenarioEvents { get; private set; }
	public ScenarioCheckEvents ScenarioCheckEvents { get; private set; }

	public ElementManager ElementManager { get; private set; }

	public AMDManager AMDManager { get; private set; }

	public CharacterManager CharacterManager { get; private set; }

	public HexIndicatorManager HexIndicatorManager { get; private set; }

	public ScenarioPhaseManager ScenarioPhaseManager { get; private set; }

	public UndoManager UndoManager { get; private set; }

	public AMDCardDeck MonsterAMDCardDeck { get; private set; }

	public static bool FastForward { get; private set; }

	public SavedScenarioProgress SavedScenarioProgress { get; private set; }

	public bool ScenarioEnded { get; private set; }

	public bool ResignRequested { get; private set; }
	public bool CheatWinRequested { get; private set; }

	public ScenarioResult ScenarioResult { get; private set; }

	public Map Map => Scenario.Map;
	public SavedScenario SavedScenario => SavedCampaign.SavedScenario;

	public static CancellationToken CancellationToken => Instance.DestroyCancellationToken;

	public override bool AdditionalLoadingCompleted => !FastForward;

	public event Action ReadyEvent;
	public event Action StartEvent;
	public static event Action<bool> FastForwardChangedEvent;
	public event Action<SavedCharacter, BattleGoalModel> BattleGoalCompletedEvent;

	public delegate void EndEventHandler(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress);

	public event EndEventHandler EndEvent;

	public override void _EnterTree()
	{
		base._EnterTree();

		SceneRequest = AppController.Instance.SceneLoader.CurrentSceneRequest as GameSceneRequest;

		if(SceneRequest == null)
		{
			SavedCampaign savedCampaign;

			if(DefaultSavedGame == null)
			{
				string path = "res://TestSaveFile.txt";
				DefaultSavedGame = FileAccess.FileExists(path) ? FileAccess.GetFileAsString(path) : string.Empty;
			}

			if(string.IsNullOrEmpty(DefaultSavedGame))
			{
				savedCampaign = SavedCampaign.Test(true);
				float characterLevelSum = savedCampaign.Characters.Sum(character => character.Level);
				savedCampaign.SetSavedScenario(new SavedScenario
				{
					Id = Guid.NewGuid(),
					AppVersion = AppController.Instance.DeviceSaveData.AppVersion,
					ScenarioModelId = ModelDB.Scenario<Scenario001>().Id.ToString(),
					//Seed = GD.RandRange(0, int.MaxValue),
					Seed = 0,
					ScenarioLevel =
						Mathf.CeilToInt((characterLevelSum / savedCampaign.Characters.Count) / 2f) +
						(AppController.Instance.CampaignOptions == null
							? 0
							: SavedCampaignOptions.DifficultyOptions.GetValue(AppController.Instance.CampaignOptions.Difficulty)),
					IsOnline = false
				});
			}
			else
			{
				savedCampaign = JsonConvert.DeserializeObject<SavedCampaign>(DefaultSavedGame, SaveManager.JsonSerializerSettings);
			}

			SceneRequest = new GameSceneRequest(savedCampaign);
		}

		FastForwardChangedEvent += OnFastForwardChanged;

		SetFastForward(true);

		SavedCampaign = SceneRequest.SavedCampaign;
		ScenarioModel = ModelDB.GetById<ScenarioModel>(SavedScenario.ScenarioModelId).ToMutable();

		SavedScenarioProgress = SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ScenarioModel);

		ReferenceManager = new ReferenceManager();
		CardManager = new CardManager();

		StateRNG = new RandomNumberGenerator();
		StateRNG.Seed = (ulong)SavedScenario.Seed;

		VisualRNG = new RandomNumberGenerator();
		VisualRNG.Randomize();

		PromptManager = new PromptManager();
		SyncedActionManager = new SyncedActionManager();

		Scenario = SceneLoader.InstantiateScene<Scenario>(ScenarioModel.ScenePath);
		AddChild(Scenario);
		Scenario.Init();

		ScenarioEvents = new ScenarioEvents();
		ScenarioCheckEvents = new ScenarioCheckEvents();

		ElementManager = new ElementManager();

		AMDManager = new AMDManager();

		CharacterManager = new CharacterManager();

		HexIndicatorManager = new HexIndicatorManager();

		ScenarioPhaseManager = new ScenarioPhaseManager();

		UndoManager = new UndoManager();

		// Create monster AMD
		List<AMDCard> amdCards = AMDCardDeck.GetDefaultDeckCards(AMDCardOwner.Monsters);
		MonsterAMDCardDeck = new AMDCardDeck(amdCards, AMDCardOwner.Monsters);

		PortraitView.Open();

		AppController.Instance.AudioController.SetBGM(ScenarioModel.BGMPath);
		AppController.Instance.AudioController.SetBGS(ScenarioModel.BGSPath);
	}

	public override void _ExitTree()
	{
		AppController.Instance.SaveGame();

		FastForwardChangedEvent -= OnFastForwardChanged;

		base._ExitTree();
	}

	public override void _Ready()
	{
		base._Ready();

		ReadyEvent?.Invoke();

		Start().Forget();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
		{
			switch(inputEventKey.Keycode)
			{
				case Key.P:
					EditorPrintSaveGame();
					break;
				case Key.Backspace:
					UndoManager.Undo();
					break;
				case Key.Escape:
					OpenMenuPopup();
					break;
				case Key.Tab:
					OpenPartyInfoPopup();
					break;
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

	private async GDTaskVoid Start()
	{
		// await GDTask.WaitWhile(() => AppController.Instance.SceneLoader.IsTransitioning, cancellationToken: DestroyCancellationToken);
		await GDTask.Yield();

		StartEvent?.Invoke();

		ScenarioPhaseManager.Play().Forget();
	}

	public static void SetFastForward(bool fastForward)
	{
		if(fastForward == FastForward)
		{
			return;
		}

		FastForward = fastForward;

		FastForwardChangedEvent?.Invoke(FastForward);
	}

	public void MarkScenarioEnded()
	{
		ScenarioEnded = true;
	}

	public async GDTask CheckEarlyEnd()
	{
		if(ResignRequested)
		{
			await AbilityCmd.Lose();
		}

		if(CheatWinRequested)
		{
			await AbilityCmd.Win();
		}
	}

	public void RequestResign()
	{
		ResignRequested = true;
	}

	public void RequestCheatWin()
	{
		CheatWinRequested = true;
	}

	public void SetScenarioResult(ScenarioResult scenarioResult)
	{
		ScenarioResult = scenarioResult;
	}

	public async GDTask EndScenario()
	{
		if(ScenarioResult == ScenarioResult.None)
		{
			Log.Error("Trying to end the scenario while the result hasn't been set.");
			return;
		}

		string scenarioModelId = SavedCampaign.SavedScenario.ScenarioModelId;

		int goldConversion = GoldConversion();
		int bonusExperience = BonusExperience();
		foreach(Character character in CharacterManager.Characters)
		{
			character.SavedCharacter.AddGold(character.ObtainedCoins * goldConversion);
			character.SavedCharacter.AddXP(character.ObtainedXP + (ScenarioResult == ScenarioResult.Win ? bonusExperience : 0));

			SavedCampaign.SanctuaryOfTheGreatOak.ReturnCards(character.SavedCharacter);

			if(ScenarioResult == ScenarioResult.Win)
			{
				BattleGoal battleGoal = character.BattleGoal;
				if(battleGoal != null && battleGoal.GivesCheckmark)
				{
					for(int i = 0; i < (int)battleGoal.Model.CheckmarkCount; i++)
					{
						character.SavedCharacter.AddCheckmark();
					}

					BattleGoalCompletedEvent?.Invoke(character.SavedCharacter, battleGoal.Model);
				}
			}
		}

		if(ScenarioResult == ScenarioResult.Win)
		{
			if(!SavedScenarioProgress.Completed)
			{
				// Give scenario rewards only if the scenario hasn't been completed before
				await AppController.Instance.GiveRewards(SavedCampaign, ScenarioModel.Rewards, true, CancellationToken);
			}

			SavedScenarioProgress.Complete();

			SavedCampaign.SetCompletedScenario(ScenarioModel);

			SavedCampaign.SavedMerchantsGuildHall.AddCompletedScenario();
		}

		if(ScenarioResult == ScenarioResult.Retry)
		{
			SavedCampaign.SetSavedScenario(new SavedScenario
			{
				Id = Guid.NewGuid(),
				AppVersion = SavedCampaign.SavedScenario.AppVersion,
				ScenarioModelId = SavedCampaign.SavedScenario.ScenarioModelId,
				Seed = GD.RandRange(0, int.MaxValue),
				ScenarioLevel = SavedCampaign.SavedScenario.ScenarioLevel,
				IsOnline = SavedCampaign.SavedScenario.IsOnline
			});
		}
		else
		{
			SavedCampaign.SetSavedScenario(null);
		}

		EndEvent?.Invoke(ScenarioResult, SavedScenarioProgress);

		// Clear any old lingering rewards and allow a new city event card to be drawn
		SavedCampaign.SavedEvents.OnScenarioEnded();
		SavedCampaign.SavedRewards.OnScenarioEnded();

		AppController.Instance.SaveGame();

		ShownIntroduction = false;

		if(ScenarioResult == ScenarioResult.Retry)
		{
			AppController.Instance.SceneLoader.RequestSceneChange(new GameSceneRequest(SavedCampaign));
		}
		else
		{
			AppController.Instance.SceneLoader.RequestSceneChange(new BetweenScenariosSceneRequest(SavedCampaign, scenarioModelId));
		}
	}

	public async GDTask OpenStoryViewIntroduction()
	{
		if(string.IsNullOrEmpty(ScenarioModel.Name) || string.IsNullOrEmpty(ScenarioModel.IntroductionText))
		{
			return;
		}

		if(ShownIntroduction || (SavedScenario.ScenarioSetupState?.Completed ?? false))
		{
			return;
		}

		SetFastForward(false);
		ShownIntroduction = true;

		string title = $"{ScenarioModel.ScenarioNumber} - {ScenarioModel.Name}";
		await AppController.Instance.StoryView.OpenAsync(title, "Introduction", ScenarioModel.IntroductionText, fadeInDuration: 0f,
			cancellationToken: CancellationToken);
	}

	public async GDTask OpenStoryViewConclusion()
	{
		if(string.IsNullOrEmpty(ScenarioModel.Name) || string.IsNullOrEmpty(ScenarioModel.ConclusionText))
		{
			return;
		}

		string title = $"{ScenarioModel.ScenarioNumber} - {ScenarioModel.Name}";
		await AppController.Instance.StoryView.OpenAsync(title, "Conclusion", ScenarioModel.ConclusionText,
			cancellationToken: CancellationToken);
	}

	public void OpenPartyInfoPopup()
	{
		this.DelayedCall(() =>
		{
			if(!AppController.Instance.PopupManager.IsPopupOpen())
			{
				AppController.Instance.PopupManager.RequestPopup(new PartyInfoPopup.Request
				{
					Characters = CharacterManager.Characters
				});
			}
		});
	}

	private void EditorPrintSaveGame()
	{
		string json = JsonConvert.SerializeObject(SavedCampaign, SaveManager.JsonSerializerSettings);
		DisplayServer.ClipboardSet(json);
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

	private void OnFastForwardChanged(bool fastForward)
	{
		if(fastForward)
		{
			_fastForwardStopwatch.Start();
		}
		else
		{
			_fastForwardStopwatch.Stop();
			Log.Write($"Fast forwarding took {_fastForwardStopwatch.ElapsedMilliseconds} milliseconds");
		}
	}

	private int GoldConversion()
	{
		int scenarioLevel = SavedScenario.ScenarioLevel;

		int value = 0;
		switch(scenarioLevel)
		{
			case 0:
			case 1:
				value = 2;
				break;
			case 2:
			case 3:
				value = 3;
				break;
			case 4:
			case 5:
				value = 4;
				break;
			case 6:
				value = 5;
				break;
			case 7:
				value = 6;
				break;
		}

		ScenarioCheckEvents.MoneyTokenValueCheck.Parameters parameters =
			ScenarioCheckEvents.MoneyTokenValueCheckEvent.Fire(
				new ScenarioCheckEvents.MoneyTokenValueCheck.Parameters(value));

		return parameters.Value;
	}

	private int BonusExperience()
	{
		int scenarioLevel = SavedScenario.ScenarioLevel;
		return scenarioLevel * 2 + 4;
	}
}