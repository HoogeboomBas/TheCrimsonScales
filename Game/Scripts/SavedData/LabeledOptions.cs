public class LabeledOptions<TValue> : LabeledOptions
{
	public LabeledOption<TValue>[] Options { get; }

	public override int OptionCount => Options.Length;

	public LabeledOptions(LabeledOption<TValue>[] options)
	{
		Options = options;
	}

	public override LabeledOption GetOption(int index)
	{
		return Options[index];
	}

	public TValue GetValue(SavedOption<int> savedOption)
	{
		return GetValue(savedOption.Value);
	}

	public TValue GetValue(int index)
	{
		return Options[index].Value;
	}
}

public abstract class LabeledOptions
{
	public abstract int OptionCount { get; }
	public abstract LabeledOption GetOption(int index);

	public string GetLabel(SavedOption<int> savedOption)
	{
		return GetLabel(savedOption.Value);
	}

	public string GetLabel(int index)
	{
		return GetOption(index).Label;
	}
}