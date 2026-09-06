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

	private readonly List<SavedReward> _duringDowntimeRewards = new List<SavedReward>();

	public BetweenScenariosSceneRequest SceneRequest { get; private set; }

	public RandomNumberGenerator RNG { get; private set; }

	public BetweenScenariosEvents Events { get; private set; }

	public List<ScenarioModel> LinkedScenarios { get; } = new List<ScenarioModel>();

	public SavedCampaign SavedCampaign => SceneRequest.SavedCampaign;

	public bool InGloomhaven => LinkedScenarios.Count == 0;

	public override void _EnterTree()
	{
		base._EnterTree();

		SceneRequest = AppController.Instance.SceneLoader.CurrentSceneRequest as BetweenScenariosSceneRequest;

		if(SceneRequest == null)
		{
			SceneRequest = new BetweenScenariosSceneRequest(SavedCampaign.Test(true));
		}

		RNG = new RandomNumberGenerator();
		RNG.Randomize();

		Events = new BetweenScenariosEvents();

		AppController.Instance.AudioController.SetBGM("res://Audio/BGM/old-tavern-cinematic-atmosphere-fairytale-273871.mp3");
		AppController.Instance.AudioController.SetBGS(null);

		SavedCampaign.CharactersChangedEvent += OnCharactersChanged;
		SubscribeCharacters();
	}

	public override void _Ready()
	{
		base._Ready();

		StartSequence().Forget();
	}

	public override void _ExitTree()
	{
		for(int i = _duringDowntimeRewards.Count - 1; i >= 0; i--)
		{
			UnsubscribeDuringDowntime(_duringDowntimeRewards[i]);
		}

		if(SavedCampaign != null)
		{
			foreach(SavedCharacter savedCharacter in SavedCampaign.Characters)
			{
				savedCharacter.SavedPersonalQuest?.Model.OnBetweenScenariosEnded(savedCharacter);
			}

			foreach(SavedPartyGoal savedPartyGoal in SavedCampaign.SavedPartyGoals.PartyGoals)
			{
				savedPartyGoal.Model.OnBetweenScenariosEnded(savedPartyGoal);
			}

			SavedCampaign.CharactersChangedEvent -= OnCharactersChanged;
		}

		UnsubscribeCharacters();

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

				if(inputEventKey.Keycode == Key.C)
				{
					foreach(SavedCharacter savedCharacter in SavedCampaign.Characters)
					{
						savedCharacter.AddCheckmark();
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
		if(!InGloomhaven && !LinkedScenarios.Contains(scenarioModel))
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Cannot start non-linked scenario",
				"To start this scenario, you first need to return to Gloomhaven. Would you like to?",
				new TextButton.Parameters("Cancel",
					() =>
					{
					}
				),
				new TextButton.Parameters("Back to Gloomhaven",
					() =>
					{
						ReturnToGloomhaven();
					},
					TextButton.ColorType.Green,
					width: 400
				)
			));

			return;
		}

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

		if(!scenarioModel.GetRequirementsMet(SavedCampaign, out string notMetMessage))
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Cannot start scenario",
				notMetMessage));

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

	public void ReturnToGloomhaven()
	{
		CancellationToken cancellationToken = DestroyCancellationToken;
		ReturnToGloomhavenSequence(cancellationToken).Forget();
	}

	public void SubscribeDuringDowntime(SavedReward reward)
	{
		reward.SubscribeDuringDowntime();
		_duringDowntimeRewards.Add(reward);
	}

	private void UnsubscribeDuringDowntime(SavedReward reward)
	{
		reward.UnsubscribeDuringDowntime();
		_duringDowntimeRewards.Remove(reward);
	}

	private async GDTaskVoid StartSequence()
	{
		CancellationToken cancellationToken = DestroyCancellationToken;

		const string introductionSeenKey = "IntroductionSeen";
		if(!SavedCampaign.GetCustomValue<bool>(introductionSeenKey))
		{
			await AppController.Instance.StoryView.OpenAsync("Introduction", null,
				"""
				Yet again you find yourselves in a tavern, but this time in a part of town most sane folk wouldn’t dream of entering. You do not frequent this area regularly, but truth be told, business has been slow and this inn is even cheaper and down-market than your normal haunts.

				Despite being in this part of town, your reputation has clearly gone before you, as it doesn’t take long before a particularly shady-looking character sidles up to you. His hood half-obscures a dirty, scarred face, his grubby cloak seems to be heavily stained with dried blood, and he twitches lightly as he starts to speak.

				“I hear you guys are open to adventure,” he mutters furtively, in a strangely-high voice. You shrug noncommittally, unsure where this is going. “I can make you rich and grant great influence if you can acquire a small trinket on my behalf?” Still dubious, you merely raise an eyebrow. But that, it seems, is enough.

				“Very good. Go to the shore of the Dark Lake, near the Watcher Mountains. There are rumors of some strange creatures there which may lead you to what you are looking for. If you are successful, I will make the necessary introductions.” Simultaneously intrigued and confused, you can’t help but ask more questions. “What is it we’re actually looking for? And what do we do when we’ve got it?” The hooded stranger half smiles. “You don’t need to know what it is. You’ll either find it, or you won’t. Look for me here if you are successful...”

				With that, the cloaked figure turns on his heel and sweeps out of the bar. You look at each other and shrug. You had no plans tomorrow anyway.
				""",
				fadeInDuration: 0f, cancellationToken: cancellationToken);

			SavedCampaign.SetCustomValue(introductionSeenKey, true);
		}

		if(SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario030>()).Completed &&
		   !SavedCampaign.HasPartyAchievement(PartyAchievement.APotionLost) &&
		   !SavedCampaign.HasPartyAchievement(PartyAchievement.TakeTheMoney))
		{
			// Make a decision! Custom reward for scenario 30
			bool? givePotion = null;
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Make a Decision!", "What will you do with the Potion?",
				[
					new TextButton.Parameters("Give the potion to Selandre", () => givePotion = true, width: 460f),
					new TextButton.Parameters("Tell Selandre the potion was destroyed", () => givePotion = false, width: 640f),
				]
			));

			await GDTask.WaitUntil(() => givePotion.HasValue, cancellationToken: cancellationToken);

			List<SavedReward> rewards;
			if(givePotion!.Value)
			{
				await AppController.Instance.StoryView.OpenAsync("Take the Money", "Given the potion to Selandre",
					"""
					Selandre’s eyes widen greedily as you hand over the instructions. “This is… not just a limited source, these are relatively simple ingredients that can be reproduced over and over,” she mutters, half to herself as she flicks through the documents. “The power contained here is incredible; with these instructions somebody could raise an entire army!” She is now ecstatic, and smiles widely.

					“Warriors, you have outstripped my wildest expectations! Here is the gold I promised you, well deserved, well deserved!” She reaches into her cloak and produces a large  velvet bag of gold coins. As she dips into it, you realize that she is is paying you personally; all this money did not come from the rough settlement you entered.

					Thanking her, and taking the gold, you can’t help hearing Dominic’s words echoing in your head as you walk away—“Destroy the potion! Don’t let it fall into the wrong hands.” Have you just done exactly that?
					""",
					fadeInDuration: 0f, cancellationToken: cancellationToken);

				rewards =
				[
					new GainGoldEachReward(70),
					new GainPartyAchievementReward(PartyAchievement.TakeTheMoney)
				];
			}
			else
			{
				await AppController.Instance.StoryView.OpenAsync("A Potion Lost", "Told Selandre the potion was destroyed",
					"""
					You tell Selandre the bad news, and she studies you for a long time. Finally she says “Well, there was always a chance that the thing that created this power would take the secrets with it to the grave. That was a tough mission, and thank you for taking it on.” She walks away and is not seen for several days. You are unsure whether you made the right decision, but you feel that you have disappointed Selandre.
					""",
					fadeInDuration: 0f, cancellationToken: cancellationToken);

				rewards =
				[
					new GainXPReward(10),
					new GainPartyAchievementReward(PartyAchievement.APotionLost)
				];
			}

			await AppController.Instance.GiveRewards(SavedCampaign, rewards, cancellationToken: cancellationToken);
		}

		await GDTask.Yield(cancellationToken);
		await GDTask.Delay(0.2f, cancellationToken: cancellationToken);

		foreach(SavedCharacter savedCharacter in SavedCampaign.Characters)
		{
			if(savedCharacter.SavedPersonalQuest != null)
			{
				await savedCharacter.SavedPersonalQuest.Model.OnBetweenScenariosStarted(savedCharacter);
			}
		}

		foreach(SavedPartyGoal savedPartyGoal in SavedCampaign.SavedPartyGoals.PartyGoals)
		{
			await savedPartyGoal.Model.OnBetweenScenariosStarted(savedPartyGoal);
		}

		foreach(SavedReward reward in SavedCampaign.SavedRewards.Rewards)
		{
			if(reward.Type == RewardType.DuringDowntime && !reward.Completed && !reward.ActivatedDuringDowntime)
			{
				SubscribeDuringDowntime(reward);
			}
		}

		if(SavedCampaign.CompletedScenarioModel != null)
		{
			foreach(ScenarioConnection connection in SavedCampaign.CompletedScenarioModel.Connections)
			{
				if(connection.Linked)
				{
					LinkedScenarios.Add(connection.To);
				}
			}
		}

		if(InGloomhaven && ModelDB.GetById<ScenarioModel>(SavedCampaign.CompletedScenarioModelId) is not SoloScenarioModel)
		{
			await ReturnToGloomhavenSequence(cancellationToken);
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

	private async GDTask ReturnToGloomhavenSequence(CancellationToken cancellationToken)
	{
		LinkedScenarios.Clear();
		SavedCampaign.SetCompletedScenario(null);

		if(SavedCampaign.SavedEvents.CanDrawCityEvent && SavedCampaign.SavedEvents.CityEventDeckIds.Count > 0)
		{
			await EventOverlay.DrawEventCard(EventType.City, cancellationToken);
		}

		if(SceneRequest.SavedCampaign.Characters.Count == 0)
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Welcome!",
				"""
				Welcome to the early access version of The Crimson Scales!

				Please create a couple of characters to get started on this campaign.
				You can do so using the button in the bottom-left corner.
				"""
			));
		}

		if(!SavedCampaign.SavedMerchantsGuildHall.Unlocked && SavedCampaign.SavedMerchantsGuildHall.CompletedScenarioCount >= 5)
		{
			await AppController.Instance.StoryView.OpenAsync("Friends in High Places", null,
				"""
				As you are walking through the Coin District, a chubby, ringed hand clasps one of you on the shoulder in a friendly manner. “Good day, adventurers!” beams the Valrath.

				You recognize him as Councilman Raksani, one of the wealthiest merchants in Gloomhaven. “You are doing a terrific job in revitalizing this city - and it is not going unnoticed.” He leans a little closer. “Myself and my associates feel that we should share our increased wealth with you. Let me know if you ever need some additional equipment, and I am sure we can arrange a small discount” he adds with a wink.
				""", cancellationToken: cancellationToken);

			SavedCampaign.SavedMerchantsGuildHall.Unlock();

			AppController.Instance.SaveGame();
		}
	}

	private async GDTaskVoid StartScenarioSequence(ScenarioModel scenarioModel)
	{
		CancellationToken cancellationToken = DestroyCancellationToken;

		if(!scenarioModel.Links.Any(link => (link.ToGloomhaven && InGloomhaven) || link.To == SavedCampaign.CompletedScenarioModel))
		{
			BetweenScenariosEvents.DrawRoadEvent.Parameters drawRoadEventParameters =
				BetweenScenariosEvents.DrawRoadEventEvent.Fire(
					new BetweenScenariosEvents.DrawRoadEvent.Parameters());

			if(drawRoadEventParameters.DrawEvent)
			{
				await EventOverlay.DrawEventCard(EventType.Road, cancellationToken);
			}
		}

		SavedCampaign savedCampaign = SavedCampaign;
		float characterLevelSum = savedCampaign.Characters.Sum(character => character.Level);
		int scenarioLevel =
			(scenarioModel is SoloScenarioModel soloScenarioModel
				? Mathf.CeilToInt(savedCampaign.Characters.First(character => character.ClassModel == soloScenarioModel.ClassModel).Level / 2f)
				: Mathf.CeilToInt((characterLevelSum / savedCampaign.Characters.Count) / 2f)) +
			(AppController.Instance.CampaignOptions == null
				? 0
				: SavedCampaignOptions.DifficultyOptions.GetValue(AppController.Instance.CampaignOptions.Difficulty));
		scenarioLevel = Mathf.Clamp(scenarioLevel, 0, 7);
		savedCampaign.SetSavedScenario(new SavedScenario()
		{
			Id = Guid.NewGuid(),
			AppVersion = AppController.Instance.DeviceSaveData.AppVersion,
			ScenarioModelId = scenarioModel.Id.ToString(),
			Seed = GD.RandRange(0, int.MaxValue),
			ScenarioLevel = scenarioLevel,
			IsOnline = false
		});

		if(AppController.Instance.CampaignSaveData != null)
		{
			AppController.Instance.CampaignSaveData.SavedCampaign = savedCampaign;
		}

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

	private void SubscribeCharacters()
	{
		foreach(SavedCharacter savedCharacter in SavedCampaign.AllCharacters)
		{
			savedCharacter.XPChangedEvent += OnXPChanged;
		}
	}

	private void UnsubscribeCharacters()
	{
		foreach(SavedCharacter savedCharacter in SavedCampaign.AllCharacters)
		{
			savedCharacter.XPChangedEvent -= OnXPChanged;
		}
	}

	private void OnCharactersChanged()
	{
		UnsubscribeCharacters();
		SubscribeCharacters();
	}

	private void OnXPChanged(SavedCharacter savedCharacter)
	{
		if(Instance == null)
		{
			Log.Warning("OnXPChanged is called in an invalid state, was an unsubscription forgotten?");
			return;
		}

		BetweenScenariosEvents.XPChangedEvent.Fire(new BetweenScenariosEvents.XPChanged.Parameters(savedCharacter));
	}
}