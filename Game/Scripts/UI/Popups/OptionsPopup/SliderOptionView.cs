using Godot;

public partial class SliderOptionView : OptionView<SliderOptionView.Parameters, int>
{
	public class Parameters : OptionViewParameters<int>
	{
		public override string ScenePath => "res://Scenes/UI/Popups/OptionsPopup/SliderOptionView.tscn";

		public Parameters(SavedOption<int> savedOption, string label)
			: base(savedOption, label)
		{
		}
	}

	[Export]
	private BetterSlider _slider;

	private bool _valueSet;

	public override void _Ready()
	{
		base._Ready();

		_slider.ValueChangedEvent += OnSliderValueChanged;
	}

	// protected override void Init()
	// {
	// 	base.Init();
	// }

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
		base.OnValueChanged(value);
	}

	private void OnSliderValueChanged(float value)
	{
		SavedOption.SetValue(Mathf.RoundToInt(value));
	}
}