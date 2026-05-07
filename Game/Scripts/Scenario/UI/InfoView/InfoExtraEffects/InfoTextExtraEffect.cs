using System;
using Godot;

public partial class InfoTextExtraEffect : InfoExtraEffect<InfoTextExtraEffect.Parameters>
{
	public class Parameters : InfoExtraEffectParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/InfoExtraEffects/InfoTextExtraEffect.tscn";

		public Func<RichTextParameters, string> GetText { get; }

		public Parameters(Func<RichTextParameters, string> getText)
		{
			GetText = getText;
		}
	}

	[Export]
	private RichTextLabel _label;

	protected override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_label.SetText(parameters.GetText(_label.GetRichTextParameters()));
	}
}