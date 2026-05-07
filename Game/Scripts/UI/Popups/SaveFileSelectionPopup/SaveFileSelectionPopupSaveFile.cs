using System.Collections.Generic;
using Godot;

public partial class SaveFileSelectionPopupSaveFile : Control
{
	[Export]
	private Label _numberLabel;

	[Export]
	private Control _emptyContainer;
	[Export]
	private Control _saveFileContainer;

	[Export]
	private PackedScene _characterScene;
	[Export]
	private Control _characterParent;

	[Export]
	private ResizingLabel _nameLabel;
	[Export]
	private Label _dateLabel;
	[Export]
	private Label _locationLabel;
	[Export]
	private Label _difficultyLabel;

	[Export]
	private BetterButton _newCampaignButton;
	[Export]
	private BetterButton _loadButton;
	[Export]
	private BetterButton _deleteButton;

	private readonly List<SaveFileSelectionPopupSaveFileCharacter> _characters = new List<SaveFileSelectionPopupSaveFileCharacter>();

	private int _index;
	private CampaignSaveData _campaignSaveData;

	public override void _Ready()
	{
		base._Ready();

		_newCampaignButton.Pressed += OnNewCampaignPressed;
		_loadButton.Pressed += OnLoadPressed;
		_deleteButton.Pressed += OnDeletePressed;
	}

	public void Init(int index)
	{
		_index = index;

		_numberLabel.SetText($"{(_index + 1).ToString()}:");

		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		_campaignSaveData = AppController.Instance.SaveManager.CampaignSaveFiles[_index].SaveData;

		SavedCampaign savedCampaign = _campaignSaveData.SavedCampaign;

		_emptyContainer.SetVisible(savedCampaign == null);
		_saveFileContainer.SetVisible(savedCampaign != null);

		foreach(SaveFileSelectionPopupSaveFileCharacter character in _characters)
		{
			character.QueueFree();
		}

		_characters.Clear();

		if(savedCampaign != null)
		{
			foreach(SavedCharacter savedCharacter in savedCampaign.Characters)
			{
				SaveFileSelectionPopupSaveFileCharacter character = _characterScene.Instantiate<SaveFileSelectionPopupSaveFileCharacter>();
				_characterParent.AddChild(character);
				character.Init(savedCharacter);
				_characters.Add(character);
			}

			_nameLabel.SetText(savedCampaign.PartyName);
			_dateLabel.SetText(_campaignSaveData.LastSaved == null ? "N.A." : _campaignSaveData.LastSaved.Value.ToString("yyyy/MM/dd"));
			_locationLabel.SetText(
				savedCampaign.SavedScenario == null
					? "Gloomhaven"
					: $"Scenario {ModelDB.GetById<ScenarioModel>(savedCampaign.SavedScenario.ScenarioModelId).ScenarioNumber}");
			_difficultyLabel.SetText(SavedCampaignOptions.DifficultyOptions.GetOption(_campaignSaveData.Options.Difficulty.Value).Label);
		}
	}

	private void OnNewCampaignPressed()
	{
		AppController.Instance.SceneLoader.RequestSceneChange(new NewCampaignSceneRequest(_index));
	}

	private void OnLoadPressed()
	{
		MainMenuController.Instance.OpenSaveFile(_index);
	}

	private void OnDeletePressed()
	{
		AppController.Instance!.PopupManager.OpenPopupOnTop(new TextPopup.Request("Are you sure?",
			$"Are you sure you want to clear this save file? This cannot be reverted!",
			new TextButton.Parameters("Cancel",
				() =>
				{
				}
			),
			new TextButton.Parameters("Delete",
				() =>
				{
					AppController.Instance.SaveManager.CampaignSaveFiles[_index].NewSaveData();
					AppController.Instance.SaveManager.SaveAll();
					UpdateVisuals();
					MainMenuController.Instance.UpdateContinueButton();
				},
				TextButton.ColorType.Red
			)
		));
	}
}