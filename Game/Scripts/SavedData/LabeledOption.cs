public class LabeledOption<TValue> : LabeledOption
{
	public override string Label { get; }
	public TValue Value { get; }

	public LabeledOption(string label, TValue value)
	{
		Label = label;
		Value = value;
	}
}

public abstract class LabeledOption
{
	public abstract string Label { get; }
}