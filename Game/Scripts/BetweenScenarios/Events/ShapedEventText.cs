using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ShapedEventText : Control
{
	[Export]
	public Curve TextShapeCurve { get; private set; }

	[Export]
	public Font Font { get; private set; }

	[Export]
	public int FontSize { get; private set; }

	[Export]
	public float LineHeight { get; private set; }

	[Export]
	public Color TextColor { get; private set; }

	[Export]
	private Control _richTextLabelParent;

	public RichTextLabel[] RichTextLabels { get; private set; }

	public void SetText(string text, bool showText)
	{
		if(TextShapeCurve == null || Font == null || _richTextLabelParent == null)
		{
			return;
		}

		if(RichTextLabels != null)
		{
			foreach(RichTextLabel richTextLabel in RichTextLabels)
			{
				richTextLabel.QueueFree();
			}
		}

		List<RichTextLabel> labels = new List<RichTextLabel>();

		string paragraphMarker = "\n";
		string[] paragraphs = text.Split([paragraphMarker], StringSplitOptions.RemoveEmptyEntries);

		foreach(string paragraph in paragraphs)
		{
			List<string> words = paragraph.Split(' ').ToList();

			string currentLine = "";
			float lineWidth = 0f;

			foreach(string word in words)
			{
				int lineIndex = labels.Count;
				float yOffset = lineIndex * LineHeight;
				float curveT = yOffset / _richTextLabelParent.Size.Y;
				lineWidth = TextShapeCurve.Sample(curveT) * _richTextLabelParent.Size.X;

				string testLine = (currentLine + " " + word).Trim();
				float testWidth = Font.GetStringSize(testLine, fontSize: FontSize).X;

				if(testWidth <= lineWidth)
				{
					currentLine = testLine;
				}
				else
				{
					// Line finished, append it
					labels.Add(CreateLabel(currentLine, false, lineWidth, labels.Count, showText));

					currentLine = word; // Start new line with this word
				}
			}

			// Add final line
			labels.Add(CreateLabel(currentLine, true, lineWidth, labels.Count, showText));

			// Empty line at the end of a paragraph
			labels.Add(CreateLabel(string.Empty, false, lineWidth, labels.Count, showText));
		}

		RichTextLabels = labels.ToArray();
	}

	private RichTextLabel CreateLabel(string line, bool finalLine, float lineWidth, int lineIndex, bool showText)
	{
		RichTextLabel label = new RichTextLabel();
		_richTextLabelParent.AddChild(label);
		if(Engine.IsEditorHint())
		{
			label.SetOwner(GetTree().EditedSceneRoot);
		}
		else
		{
			label.SetOwner(GetTree().Root);
		}

		label.SetPosition(new Vector2((_richTextLabelParent.Size.X - lineWidth) / 2f, lineIndex * LineHeight));
		label.SetSize(new Vector2(lineWidth, LineHeight));
		label.SetHorizontalAlignment(finalLine ? HorizontalAlignment.Left : HorizontalAlignment.Fill);
		label.SetSelfModulate(TextColor);

		label.AddThemeFontOverride("normal_font", Font);
		label.AddThemeFontSizeOverride("normal_font_size", FontSize);
		label.SetText(line);
		label.SetVisibleCharactersBehavior(TextServer.VisibleCharactersBehavior.CharsAfterShaping);
		label.SetFitContent(true);

		if(!showText)
		{
			label.SetVisibleCharacters(0);
		}

		return label;
	}

	private static string WrapTextToWidths(string text, float[] widths, Font font, int fontSize)
	{
		List<string> words = text.Split(' ').ToList();
		int lineIndex = 0;

		string currentLine = "";
		string result = "";

		foreach(string word in words)
		{
			if(lineIndex >= widths.Length)
			{
				break;
			}

			string testLine = (currentLine + " " + word).Trim();
			float testWidth = font.GetStringSize(testLine, fontSize: fontSize).X;

			if(testWidth <= widths[lineIndex])
			{
				currentLine = testLine;
			}
			else
			{
				// Line finished, append it
				result += currentLine + "\n";
				lineIndex++;

				if(lineIndex >= widths.Length)
				{
					break;
				}

				currentLine = word; // Start new line with this word
			}
		}

		// Add final line
		if(lineIndex < widths.Length)
		{
			result += currentLine;
		}

		return result;
	}
}