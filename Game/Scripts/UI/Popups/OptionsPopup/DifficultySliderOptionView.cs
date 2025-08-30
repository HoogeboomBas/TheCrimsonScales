using Godot;

public partial class DifficultySliderOptionView : OptionView<DifficultySliderOptionView.Parameters, int>
{
	public class Parameters : OptionViewParameters<int>
	{
		public override string ScenePath => "res://Scenes/UI/Popups/OptionsPopup/DifficultySliderOptionView.tscn";

		public Parameters(SavedOption<int> savedOption, string label)
			: base(label, savedOption)
		{
		}
	}

	[Export]
	private BetterSlider _slider;
	[Export]
	private Label _valueLabel;

	private bool _valueSet;

	public override void _Ready()
	{
		base._Ready();

		_slider.ValueChangedEvent += OnSliderValueChanged;
	}

	public override void OnOpen()
	{
		base.OnOpen();

		if(_valueSet)
		{
			return;
		}

		_valueSet = true;

		this.DelayedCall(() =>
		{
			_slider.SetValue(SavedOption.Value);
		});
	}

	protected override void OnValueChanged(int value)
	{
		string difficultyName = value switch
		{
			-1 => "Easy",
			0 => "Normal",
			1 => "Hard",
			2 => "Very Hard",
			_ => string.Empty
		};

		_valueLabel.Text = $"{difficultyName} ({(value >= 0 ? "+" : string.Empty)}{value})";
	}

	private void OnSliderValueChanged(float value)
	{
		SavedOption.SetValue(Mathf.RoundToInt(value));
	}
}