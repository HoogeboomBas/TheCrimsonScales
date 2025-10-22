using Godot;

public partial class BetweenScenariosController : SceneController<BetweenScenariosController>
{
	[Export]
	public BetweenScenariosSidePanel BetweenScenariosSidePanel { get; private set; }

	[Export]
	public BetweenScenariosCharacterPortraitManager CharacterPortraitManager { get; private set; }

	[Export]
	public ScenarioFlowchart ScenarioFlowchart { get; private set; }

	public BetweenScenariosSceneRequest SceneRequest { get; private set; }

	public SavedCampaign SavedCampaign => SceneRequest.SavedCampaign;

	public override void _EnterTree()
	{
		base._EnterTree();

		SceneRequest = AppController.Instance.SceneLoader.CurrentSceneRequest as BetweenScenariosSceneRequest;

		if(SceneRequest == null)
		{
			SceneRequest = new BetweenScenariosSceneRequest(SavedCampaign.Test());
		}

		if(SceneRequest.SavedCampaign.Characters.Count == 0)
		{
			this.DelayedCall(() =>
			{
				AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Welcome!",
					"Welcome to the very early access version of The Crimson Scales!\nPlease create a couple of characters to get started on this campaign. " +
					"You can do so using the button in the bottom-left corner."
				));
			}, 0.5f);
		}

		if(SceneRequest.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario010>()).Completed &&
		   SceneRequest.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario013>()).Completed &&
		   SceneRequest.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(ModelDB.Scenario<Scenario014>()).Completed)
		{
			this.DelayedCall(() =>
			{
				AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("End of Demo",
					"Thank you for playing this demo of The Crimson Scales!\nHope you had fun!" +
					"\nAny and all feedback is very welcome. Please do not hesitate to let us know your thoughts."
				));
			});
		}

		AppController.Instance.AudioController.SetBGM("res://Audio/BGM/old-tavern-cinematic-atmosphere-fairytale-273871.mp3");
		AppController.Instance.AudioController.SetBGS(null);
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

			if(inputEventKey.Keycode == Key.X && OS.IsDebugBuild())
			{
				foreach(SavedCharacter savedCharacter in SavedCampaign.Characters)
				{
					savedCharacter.AddXP(30);
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