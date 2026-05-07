using System;
using System.Collections.Generic;
using Godot;

public partial class SaveFileSelectionPopup : Popup<SaveFileSelectionPopup.Request>
{
	public class Request : PopupRequest
	{
	}

	[Export]
	private PackedScene _saveFileScene;
	[Export]
	private Control _saveFileParent;

	[Export]
	private BetterButton _closeButton;

	private readonly List<SaveFileSelectionPopupSaveFile> _saveFiles = new List<SaveFileSelectionPopupSaveFile>();

	public override void _Ready()
	{
		base._Ready();

		_closeButton.Pressed += OnClosePressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		for(int i = 0; i < AppController.Instance.SaveManager.CampaignSaveFiles.Count; i++)
		{
			SaveFileSelectionPopupSaveFile saveFile = _saveFileScene.Instantiate<SaveFileSelectionPopupSaveFile>();
			_saveFileParent.AddChild(saveFile);
			saveFile.Init(i);
			_saveFiles.Add(saveFile);
		}
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(SaveFileSelectionPopupSaveFile saveFile in _saveFiles)
		{
			saveFile.QueueFree();
		}

		_saveFiles.Clear();
	}

	private void OnClosePressed()
	{
		Close();
	}
}