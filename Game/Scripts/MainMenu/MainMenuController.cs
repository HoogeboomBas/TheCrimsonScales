using System.Linq;
using Godot;

public partial class MainMenuController : SceneController<MainMenuController>
{
	[Export]
	private BetterButton _continueButton;
	[Export]
	private BetterButton _playButton;
	[Export]
	private BetterButton _provingGroundsButton;
	[Export]
	private BetterButton _optionsButton;
	[Export]
	private BetterButton _exitButton;

	private MainMenuSceneRequest _sceneRequest;

	public override void _EnterTree()
	{
		_sceneRequest = AppController.Instance.SceneLoader.CurrentSceneRequest as MainMenuSceneRequest;

		if(_sceneRequest == null)
		{
			_sceneRequest = new MainMenuSceneRequest();
		}

		UpdateContinueButton();

		_continueButton.Pressed += OnContinuePressed;
		_playButton.Pressed += OnPlayPressed;
		_provingGroundsButton.Pressed += OnProvingGroundsPressed;
		_optionsButton.Pressed += OnOptionsPressed;
		_exitButton.Pressed += OnExitPressed;

		_exitButton.GetParent<Control>().SetVisible(Platform.DeskTop);
	}

	public override void _Ready()
	{
		base._Ready();

		AppController.Instance.AudioController.SetBGM("res://Audio/BGM/Call to Adventure FULL LOOP TomMusic.ogg");
		AppController.Instance.AudioController.SetBGS(null);

		AppController.Instance.SaveManager.SetCampaignIndex(-1);
	}

	public void OpenSaveFile(int index)
	{
		AppController.Instance.DeviceSaveData.LastCampaignIndex = index;
		AppController.Instance.SaveManager.SaveCampaignAndDevice();
		AppController.Instance.SaveManager.SetCampaignIndex(AppController.Instance.DeviceSaveData.LastCampaignIndex);
		SavedCampaign savedCampaign = AppController.Instance.CampaignSaveData.SavedCampaign;
		if(savedCampaign.SavedScenario == null)
		{
			AppController.Instance.SceneLoader.RequestSceneChange(new BetweenScenariosSceneRequest(savedCampaign));
		}
		else
		{
			AppController.Instance.SceneLoader.RequestSceneChange(new GameSceneRequest(savedCampaign));
		}
	}

	public void UpdateContinueButton()
	{
		bool continueAvailable =
			AppController.Instance.DeviceSaveData.LastCampaignIndex >= 0 &&
			AppController.Instance.SaveManager.CampaignSaveFiles[AppController.Instance.DeviceSaveData.LastCampaignIndex].SaveData.SavedCampaign !=
			null;
		_continueButton.GetParent<Control>().SetVisible(continueAvailable);
	}

	private void OnContinuePressed()
	{
		OpenSaveFile(AppController.Instance.DeviceSaveData.LastCampaignIndex);
	}

	private void OnPlayPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new SaveFileSelectionPopup.Request()
		{
		});
	}

	private void OnProvingGroundsPressed()
	{
		SavedCampaign campaign = SavedCampaign.New("Proving Grounds", StartingGroup.Militants);
		campaign.UnlockClass(ModelDB.Class<BruiserModel>());
		campaign.UnlockClass(ModelDB.Class<ThornreaperModel>());
		campaign.AddCharacter(ModelDB.Class<BruiserModel>(), null, "Bruiser");
		campaign.AddCharacter(ModelDB.Class<ThornreaperModel>(), null, "Thornreaper");
		SavedCharacter bruiser = campaign.Characters.LastOrDefault();

		campaign.SavedScenarioProgresses.ScenarioProgresses.Clear();
		SavedScenarioProgress provingGrounds = new SavedScenarioProgress();
		provingGrounds.Discover();
		campaign.SavedScenarioProgresses.ScenarioProgresses.Add(
			ModelDB.GetId<Scenario900>().ToString(), provingGrounds);
		int index = 0;
		AppController.Instance.SaveManager.SetCampaignIndex(index);
		AppController.Instance.DeviceSaveData.LastCampaignIndex = index;
		AppController.Instance.CampaignSaveData.SavedCampaign = campaign;
		AppController.Instance.SaveManager.SaveCampaignAndDevice();

		AppController.Instance.SceneLoader.RequestSceneChange(
			new BetweenScenariosSceneRequest(campaign));
	}

	private void OnOptionsPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new OptionsPopup.Request());
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
