using Fractural.Tasks;
using Godot;

public partial class ErrorPopup : Popup<ErrorPopup.Request>
{
	public class Request : PopupRequest
	{
		public string ErrorFilePath { get; init; }
		public string FullErrorMessage { get; init; }
	}

	[Export]
	private RichTextLabel _fileLabel;
	[Export]
	private BetterButton _fileButton;

	[Export]
	private BetterButton _copyErrorToClipboardButton;

	[Export]
	private BetterButton _returnToTownButton;
	[Export]
	private BetterButton _undoButton;
	[Export]
	private BetterButton _toMainMenuButton;

	public override void _Ready()
	{
		base._Ready();

		_fileButton.Pressed += OnFilePressed;

		_copyErrorToClipboardButton.Pressed += OnCopyErrorToClipboardPressed;

		_returnToTownButton.Pressed += OnReturnToTownPressed;
		_undoButton.Pressed += OnUndoPressed;
		_toMainMenuButton.Pressed += ToMainMenuPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		int errorFilePathHalfSize = 35;
		string errorFilePath = PopupRequest.ErrorFilePath.Length > 2 * errorFilePathHalfSize + 4
			? $"{PopupRequest.ErrorFilePath.Substring(0, errorFilePathHalfSize)}...{PopupRequest.ErrorFilePath.Substring(PopupRequest.ErrorFilePath.Length - errorFilePathHalfSize, errorFilePathHalfSize)}"
			: PopupRequest.ErrorFilePath;
		_fileLabel.SetText(errorFilePath);

		_returnToTownButton.GetParent<Control>().SetVisible(GameController.Instance != null);
		_undoButton.GetParent<Control>().SetVisible(GameController.Instance != null);
	}

	private void OnFilePressed()
	{
	}

	private void OnCopyErrorToClipboardPressed()
	{
		DisplayServer.ClipboardSet(PopupRequest.FullErrorMessage);
	}

	private void OnReturnToTownPressed()
	{
		ReturnToTown().Forget();
	}

	private async GDTaskVoid ReturnToTown()
	{
		GameController.Instance.SetScenarioResult(ScenarioResult.Loss);
		Log.ResetHasLoggedError();
		await GameController.Instance.EndScenario();
	}

	private void OnUndoPressed()
	{
		GameController.Instance.UndoManager.Undo();
		Log.ResetHasLoggedError();
	}

	private void ToMainMenuPressed()
	{
		AppController.Instance.SceneLoader.RequestSceneChange(new MainMenuSceneRequest());
		Log.ResetHasLoggedError();
	}
}