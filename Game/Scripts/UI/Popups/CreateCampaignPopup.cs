using Godot;

public partial class CreateCampaignPopup : Popup<CreateCampaignPopup.Request>
{
	public class Request : PopupRequest
	{
	}

	[Export]
	private LineEdit _nameLineEdit;

	[Export]
	private BetterButton _cancelButton;
	[Export]
	private BetterButton _confirmButton;

	public override void _Ready()
	{
		base._Ready();

		_nameLineEdit.TextChanged += OnNameChanged;

		_cancelButton.Pressed += OnCancelPressed;
		_confirmButton.Pressed += OnConfirmPressed;

		OnNameChanged(_nameLineEdit.Text);
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		_nameLineEdit.SetText(string.Empty);
		OnNameChanged(_nameLineEdit.Text);
	}

	private void OnNameChanged(string newText)
	{
		_confirmButton.SetEnabled(!string.IsNullOrEmpty(newText));
	}

	private void OnCancelPressed()
	{
		Close();
	}

	private void OnConfirmPressed()
	{
		Close();

		AppController.Instance.SaveFile.SaveData.SavedCampaign = SavedCampaign.New(_nameLineEdit.Text, StartingGroup.Militants);

		AppController.Instance.SaveFile.Save();

		AppController.Instance.SceneLoader.RequestSceneChange(
			new BetweenScenariosSceneRequest(AppController.Instance.SaveFile.SaveData.SavedCampaign));
	}
}