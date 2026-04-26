using System.Text;

public static class TextHelper
{
	public delegate string LabelTextDelegate(RichTextParameters textParameters);

	public static string Prettify(string text)
	{
		text = text
			.Replace("“", "\"")
			.Replace("”", "\"")
			.Replace("‘", "'")
			.Replace("’", "'");

		text = SmartQuotes(text);
		return text;
	}

	private static string SmartQuotes(string text)
	{
		StringBuilder sb = new StringBuilder(text.Length);

		bool openDouble = true;

		for(int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			char prev = i > 0 ? text[i - 1] : '\0';
			char next = i < text.Length - 1 ? text[i + 1] : '\0';

			if(c == '"')
			{
				sb.Append(openDouble ? '“' : '”');
				openDouble = !openDouble;
				continue;
			}

			if(c == '\'')
			{
				bool prevIsLetter = char.IsLetterOrDigit(prev);
				bool nextIsLetter = char.IsLetterOrDigit(next);

				// Apostrophe (don't, what's, John's)
				if(prevIsLetter && nextIsLetter)
				{
					sb.Append('’');
				}
				// Opening quote (start of quoted phrase)
				else if(char.IsWhiteSpace(prev) || prev == '\0' || "([{\n".Contains(prev))
				{
					sb.Append('‘');
				}
				// Otherwise closing quote
				else
				{
					sb.Append('’');
				}

				continue;
			}

			sb.Append(c);
		}

		return sb.ToString();
	}
}