using Godot;

public partial class InfoTextExtraEffect : InfoExtraEffect<InfoTextExtraEffect.Parameters>
{
	public class Parameters : InfoExtraEffectParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/InfoExtraEffects/InfoTextExtraEffect.tscn";

		public string Text { get; }

		public Parameters(string text)
		{
			Text = text;
		}
	}

	[Export]
	private RichTextLabel _label;

	protected override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_label.SetText(parameters.Text);
	}
}