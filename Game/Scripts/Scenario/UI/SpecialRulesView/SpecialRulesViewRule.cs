using Godot;

public partial class SpecialRulesViewRule : Control
{
	[Export]
	private RichTextLabel _label;

	public ScenarioRule Rule { get; private set; }

	public void Init(ScenarioRule rule)
	{
		Rule = rule;

		Rule.TextChangedEvent += OnTextChanged;

		UpdateText();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(Rule != null)
		{
			Rule.TextChangedEvent -= OnTextChanged;
		}
	}

	private void UpdateText()
	{
		RichTextParameters textParameters = _label.GetRichTextParameters();
		_label.SetText(Rule.GetLabelText(textParameters));
	}

	private void OnTextChanged(ScenarioRule rule)
	{
		UpdateText();
	}
}