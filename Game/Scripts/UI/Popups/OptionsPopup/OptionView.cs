using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public abstract partial class OptionView<TParameters, TValue> : OptionViewBase
	where TParameters : OptionViewParameters<TValue>
{
	private Label _label;

	private TParameters _parameters;

	protected SavedOption<TValue> SavedOption => _parameters.SavedOption;

	public override void _Ready()
	{
		base._Ready();

		_label = GetNode<Label>("LabelContainer/Label");
	}

	public sealed override void Init(OptionViewParameters parameters)
	{
		_parameters = (TParameters)parameters;

		Init();

		_label.SetText(parameters.Label);
	}

	protected virtual void Init()
	{
		SavedOption.ValueChangedEvent += OnValueChanged;
		OnValueChanged(SavedOption.Value);
	}

	public override void Destroy()
	{
		this.TweenScale(0.5f, 0.15f).SetEasing(Easing.InBack).OnComplete(QueueFree).Play();
	}

	protected virtual void OnValueChanged(TValue value)
	{
	}
}