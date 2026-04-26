using System;
using Godot;

public class ScenarioRule
{
	private TextHelper.LabelTextDelegate _getLabelText;

	public int Order { get; }

	public bool Removed { get; private set; }

	public event Action<ScenarioRule> TextChangedEvent;
	public event Action<ScenarioRule> TextRemovedEvent;

	public ScenarioRule(TextHelper.LabelTextDelegate getLabelText, int order = 0)
	{
		_getLabelText = getLabelText;
		Order = order;
	}

	public string GetLabelText(RichTextParameters textParameters) => _getLabelText(textParameters);

	public void SetText(TextHelper.LabelTextDelegate getTextLabel)
	{
		_getLabelText = getTextLabel;

		TextChangedEvent?.Invoke(this);
	}

	public void Remove()
	{
		if(Removed)
		{
			Log.Warning("Removing a rule that was already removed.");
			return;
		}

		Removed = true;

		TextRemovedEvent?.Invoke(this);
	}
}