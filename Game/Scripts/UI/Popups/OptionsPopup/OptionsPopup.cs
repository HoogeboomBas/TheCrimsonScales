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

		SavedOptions options = AppController.Instance.SaveFile.SaveData.Options;

		AddSliderOption(options.BGMVolume, "Music");
		AddSliderOption(options.BGSVolume, "Ambience");
		AddSliderOption(options.SFXVolume, "Sound Effects");

		if(!Platform.DeskTop)
		{
			AddCheckmarkOption(options.VibrationsEnabled, "Vibrations");
		}

		AddCheckmarkOption(options.AnimatedCharacters, "Animated Characters");

		AddOption(new DifficultySliderOptionView.Parameters(options.Difficulty, "Difficulty"));
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		_confirmButton.SetEnabled(true, false);

		foreach(OptionViewBase option in _options)
		{
			option.OnOpen();
		}
	}

	protected override void OnClose()
	{
		base.OnClose();

		AppController.Instance.SaveFile.Save();
	}

	private void AddCheckmarkOption(SavedOption<bool> option, string label)
	{
		AddOption(new CheckmarkOptionView.Parameters(option, label));
	}

	private void AddSliderOption(SavedOption<int> option, string label)
	{
		AddOption(new SliderOptionView.Parameters(option, label));
	}

	private void AddOption(OptionViewParameters parameters)
	{
		PackedScene scene = ResourceLoader.Load<PackedScene>(parameters.ScenePath);
		OptionViewBase optionView = scene.Instantiate<OptionViewBase>();
		_optionViewParent.AddChild(optionView);
		optionView.Init(parameters);
		_options.Add(optionView);
	}

	private void OnConfirmPressed()
	{
		Close();
	}
}