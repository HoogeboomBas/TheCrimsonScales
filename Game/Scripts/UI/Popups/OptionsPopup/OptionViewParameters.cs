public abstract class OptionViewParameters<T> : OptionViewParameters
{
	public SavedOption<T> SavedOption { get; }

	public OptionViewParameters(SavedOption<T> savedOption, string label, bool enabled)
	{
		SavedOption = savedOption;
		Label = label;
		Enabled = enabled;
	}
}

public abstract class OptionViewParameters
{
	public string Label { get; protected set; }
	public bool Enabled { get; protected set; }

	public abstract string ScenePath { get; }
}