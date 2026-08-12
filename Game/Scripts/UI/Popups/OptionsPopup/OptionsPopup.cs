using System.Collections.Generic;
using Godot;

public partial class OptionsPopup : Popup<OptionsPopup.Request>
{
	public class Request : PopupRequest
	{
	}

	[Export]
	private BetterButton _confirmButton;

	[Export]
	private Control _optionViewParent;

	private readonly List<OptionViewBase> _options = new List<OptionViewBase>();

	public override void _Ready()
	{
		base._Ready();

		_confirmButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		_confirmButton.SetEnabled(true, false);

		SavedDeviceOptions deviceOptions = AppController.Instance.DeviceOptions;
		SavedCampaignOptions campaignOptions = AppController.Instance.CampaignOptions;

		AddSliderOption(deviceOptions.BGMVolume, "Music");
		AddSliderOption(deviceOptions.BGSVolume, "Ambience");
		AddSliderOption(deviceOptions.SFXVolume, "Sound Effects");

		if(Platform.DeskTop)
		{
			DisplayServer.WindowMode windowMode = DisplayServer.WindowGetMode();
			deviceOptions.FullScreen.SetValue(windowMode == DisplayServer.WindowMode.Fullscreen);

			AddCheckmarkOption(deviceOptions.FullScreen, "Full Screen");
		}
		else
		{
			AddCheckmarkOption(deviceOptions.VibrationsEnabled, "Vibrations");
		}

		AddCheckmarkOption(deviceOptions.ScreenShakeEnabled, "Screen Shake");

		AddLabeledSliderOption(deviceOptions.GameplaySpeed, "Gameplay Speed", SavedDeviceOptions.SpeedOptions);

		AddCheckmarkOption(deviceOptions.AnimatedCharacters, "Animated Characters");

		if(campaignOptions != null)
		{
			AddLabeledSliderOption(campaignOptions.Difficulty, "Difficulty", SavedCampaignOptions.DifficultyOptions, GameController.Instance == null);
		}

		foreach(OptionViewBase option in _options)
		{
			option.OnOpen();
		}
	}

	protected override void OnClose()
	{
		base.OnClose();

		AppController.Instance.SaveManager.SaveCampaignAndDevice();
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(OptionViewBase option in _options)
		{
			option.QueueFree();
		}

		_options.Clear();
	}

	private void AddCheckmarkOption(SavedOption<bool> option, string label, bool enabled = true)
	{
		AddOption(new CheckmarkOptionView.Parameters(option, label, enabled));
	}

	private void AddSliderOption(SavedOption<int> option, string label, bool enabled = true)
	{
		AddOption(new SliderOptionView.Parameters(option, label, enabled));
	}

	private void AddLabeledSliderOption(SavedOption<int> option, string label, LabeledOptions options, bool enabled = true)
	{
		AddOption(new LabeledSliderOptionView.Parameters(option, label, options, enabled));
	}

	private void AddOption(OptionViewParameters parameters)
	{
		OptionViewBase optionView = SceneLoader.InstantiateScene<OptionViewBase>(parameters.ScenePath);
		_optionViewParent.AddChild(optionView);
		optionView.Init(parameters);
		_options.Add(optionView);
	}

	private void OnConfirmPressed()
	{
		Close();
	}
}