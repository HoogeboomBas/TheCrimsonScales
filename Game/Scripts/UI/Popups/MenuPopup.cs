using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public partial class MenuPopup : Popup<MenuPopup.Request>
{
	public class Request : PopupRequest
	{
	}

	[Export]
	private BetterButton _resumeButton;
	[Export]
	private BetterButton _optionsButton;
	[Export]
	private BetterButton _undoTurnButton;
	[Export]
	private BetterButton _undoRoundButton;
	[Export]
	private BetterButton _resignButton;
	[Export]
	private BetterButton _winButton;
	[Export]
	private BetterButton _copyToClipboardButton;
	[Export]
	private BetterButton _exitButton;

	public override void _Ready()
	{
		base._Ready();

		_resumeButton.Pressed += OnResumePressed;
		_optionsButton.Pressed += OnOptionsPressed;
		_undoTurnButton.Pressed += OnUndoTurnPressed;
		_undoRoundButton.Pressed += OnUndoRoundPressed;
		_resignButton.Pressed += OnResignPressed;
		_winButton.Pressed += OnWinPressed;
		_copyToClipboardButton =
			GetNode<BetterButton>("Panel/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/Content/CopyToClipboardButton/BetterButton");
		_copyToClipboardButton.Pressed += OnCopyToClipboardPressed;
		_exitButton.Pressed += OnExitPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		_undoTurnButton.GetParent<Control>().SetVisible(GameController.Instance != null && GameController.Instance.UndoManager.CanUndoTurn);
		_undoRoundButton.GetParent<Control>().SetVisible(GameController.Instance != null && GameController.Instance.UndoManager.CanUndoRound);
		_resignButton.GetParent<Control>().SetVisible(GameController.Instance != null);
		_winButton.GetParent<Control>().SetVisible(GameController.Instance != null);
		_copyToClipboardButton.GetParent<Control>().SetVisible(GameController.Instance != null);
	}

	private void OnResumePressed()
	{
		Close();
	}

	private void OnOptionsPressed()
	{
		Close();

		AppController.Instance.PopupManager.OpenPopupOnTop(new OptionsPopup.Request());
	}

	private void OnUndoTurnPressed()
	{
		Close();

		GameController.Instance.UndoManager.UndoTurn();
	}

	private void OnUndoRoundPressed()
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Undo Round",
			"Are you sure you undo to the start of the past round?",
			new TextButton.Parameters("Cancel", () =>
			{
			}),
			new TextButton.Parameters("Undo", () =>
			{
				Close();

				GameController.Instance.UndoManager.UndoRound();
			}, TextButton.ColorType.Red)
		));
	}

	private void OnCopyToClipboardPressed()
	{
		Close();

		Formatting oldFormatting = SaveManager.JsonSerializerSettings.Formatting;
		SaveManager.JsonSerializerSettings.Formatting = Formatting.Indented;
		string json = JsonConvert.SerializeObject(GameController.Instance.SavedCampaign, SaveManager.JsonSerializerSettings);
		SaveManager.JsonSerializerSettings.Formatting = oldFormatting;
		//GD.Print(json);
		DisplayServer.ClipboardSet(json);
	}

	private void OnResignPressed()
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Resign",
			"Are you sure you want to resign?\nYou will lose the scenario.",
			new TextButton.Parameters("Cancel", () =>
			{
			}),
			new TextButton.Parameters("Resign", () =>
			{
				Close();

				GameController.Instance.RequestResign();
			}, TextButton.ColorType.Red)
		));
	}

	private void OnWinPressed()
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Win", "Are you sure you want to cheat a win?",
			new TextButton.Parameters("Cancel", () =>
			{
			}),
			new TextButton.Parameters("Win", () =>
			{
				Close();

				GameController.Instance.RequestCheatWin();
			}, TextButton.ColorType.Green)
		));
	}

	private void OnExitPressed()
	{
		List<TextButton.Parameters> textButtonParametersList = new List<TextButton.Parameters>();
		textButtonParametersList.Add(new TextButton.Parameters("Cancel", () =>
		{
		}));
		textButtonParametersList.Add(new TextButton.Parameters("Main Menu", () =>
		{
			AppController.Instance.SceneLoader.RequestSceneChange(new MainMenuSceneRequest());
		}));

		if(Platform.DeskTop)
		{
			textButtonParametersList.Add(new TextButton.Parameters("Exit Game", () =>
			{
				GetTree().Quit();
			}));
		}

		AppController.Instance.PopupManager.OpenPopupOnTop(
			new TextPopup.Request("Save and Exit", "Would you like to exit?", textButtonParametersList.ToArray()));
	}
}