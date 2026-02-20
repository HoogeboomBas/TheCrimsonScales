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
		return new RichTextParameters(richTextLabel.GetThemeFontSize("normal_font_size"), richTextLabel.SelfModulate);
	}
}