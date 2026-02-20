using Godot;

public partial class CharacterInfoPopup : Popup<CharacterInfoPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCampaign SavedCampaign { get; init; }
		public SavedCharacter SavedCharacter { get; init; }
	}

	[Export]
	private TextureRect _matFrontTexture;

	[Export]
	private PersonalQuestProgressView _personalQuestProgressView;
	[Export]
	private TextButton _retireButton;

	[Export]
	private LineEdit _nameLineEdit;

	[Export]
	private Control _header;

	[Export]
	private BetterButton _cancelButton;
	[Export]
	private BetterButton _confirmButton;
	[Export]
	private BetterButton _deleteButton;

	public override void _Ready()
	{
		base._Ready();

		_retireButton.Init("Retire", OnRetirePressed, TextButton.ColorType.Green);

		_nameLineEdit.TextChanged += OnNameChanged;

		_cancelButton.Pressed += OnCancelPressed;
		_confirmButton.Pressed += OnConfirmPressed;
		_deleteButton.Pressed += OnDeletePressed;

		OnNameChanged(_nameLineEdit.Text);
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		_matFrontTexture.Texture = PopupRequest.SavedCharacter.ClassModel.MatFrontTexture;

		_personalQuestProgressView.Init(PopupRequest.SavedCharacter);
		_retireButton.SetVisible(PopupRequest.SavedCharacter.GetCanRetire(PopupRequest.SavedCampaign));

		_nameLineEdit.SetText(PopupRequest.SavedCharacter.Name);
		OnNameChanged(_nameLineEdit.Text);
	}

	private void OnRetirePressed()
	{
		if(!PopupRequest.SavedCharacter.GetCanRetire(PopupRequest.SavedCampaign))
		{
			return;
		}

		BetweenScenariosController.Instance?.RetireCharacter(PopupRequest.SavedCharacter, PopupRequest.SavedCampaign);

		Close();
	}

	private void OnNameChanged(string newText)
	{
		_confirmButton.SetEnabled(!string.IsNullOrEmpty(newText) && newText != PopupRequest.SavedCharacter.Name);
	}

	private void OnCancelPressed()
	{
		Close();
	}

	private void OnConfirmPressed()
	{
		PopupRequest.SavedCharacter.SetName(_nameLineEdit.Text);

		Close();
	}

	private void OnDeletePressed()
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Are you sure?",
			"Are you sure you want to delete this character?\nThis can not be undone!",
			new TextButton.Parameters("Cancel",
				() =>
				{
				}
			),
			new TextButton.Parameters("Delete",
				() =>
				{
					PopupRequest.SavedCampaign.DeleteCharacter(PopupRequest.SavedCharacter);

					AppController.Instance.SaveFile.Save();

					Close();
				},
				TextButton.ColorType.Red
			)
		));
	}
}