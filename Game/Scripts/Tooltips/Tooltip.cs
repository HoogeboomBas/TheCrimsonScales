public class Tooltip
{
	public string Name { get; }
	public string IconPath { get; }
	public string Category { get; }
	public TextHelper.LabelTextDelegate GetText { get; }

	public Tooltip(string name, string iconPath, string category, TextHelper.LabelTextDelegate getText)
	{
		Name = name;
		IconPath = iconPath;
		Category = category;
		GetText = getText;
	}
}