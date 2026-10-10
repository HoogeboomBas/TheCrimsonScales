public class Tooltip
{
	public string Name { get; }
	public string IconPath { get; }
	public string Category { get; }
	public TextHelper.LabelTextDelegate GetLabelText { get; }

	public Tooltip(string name, string iconPath, string category, TextHelper.LabelTextDelegate getLabelText)
	{
		Name = name;
		IconPath = iconPath;
		Category = category;
		GetLabelText = getLabelText;
	}
}