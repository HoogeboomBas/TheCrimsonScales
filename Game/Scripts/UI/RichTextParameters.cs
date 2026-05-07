using Godot;

public struct RichTextParameters
{
	public int FontSize { get; }
	public Color Color { get; }

	public RichTextParameters(int fontSize, Color color)
	{
		FontSize = fontSize;
		Color = color;
	}
}

public static class RichTextLabelExtensionMethods
{
	public static RichTextParameters GetRichTextParameters(this RichTextLabel richTextLabel)
	{
		Color color = richTextLabel.GetThemeColor("default_color");
		int fontSize = richTextLabel.GetThemeFontSize("normal_font_size");
		return new RichTextParameters(fontSize, color);
	}
}