using Godot;

public partial class CheckmarkOptionView : OptionView<CheckmarkOptionView.Parameters, bool>
{
	public class Parameters : OptionViewParameters<bool>
	{
		public override string ScenePath => "res://Scenes/UI/Popups/OptionsPopup/CheckmarkOptionView.tscn";

		public Parameters(SavedOption<bool> savedOption, string label)
			: base(savedOption, label)
		{
		}
	}

	[Export]
	private Control _checkmark;
	[Export]
	private BetterButton _button;

	public override void _Ready()
	{
		base._Ready();

		_button.Pressed += OnButtonPressed;
	}

	// protected override void Init()
	// {
	// 	base.Init();
	// }

	protected override void OnValueChanged(bool value)
	{
		base.OnValueChanged(value);

		_checkmark.SetVisible(value);
	}

	private void OnButtonPressed()
	{
		SavedOption.SetValue(!SavedOption.Value);
	}
}