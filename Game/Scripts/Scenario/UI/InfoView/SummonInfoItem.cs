using Godot;

public partial class SummonInfoItem : FigureInfoItem<SummonInfoItem.Parameters>
{
	public class Parameters(Summon hexObject) : FigureInfoItemParameters(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/SummonInfoItem.tscn";

		public Summon Summon { get; } = hexObject;
	}

	[Export]
	private Label _moveLabel;
	[Export]
	private Label _attackLabel;
	[Export]
	private Label _rangeLabel;

	private Summon _summon;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_summon = parameters.Summon;

		_portraitTexture.SetTexture(_summon.Texture);
		_portraitBorder.SetSelfModulate(_summon.OutlineColor);

		_moveLabel.SetText(_summon.Stats.Move?.ToString() ?? "-");
		_attackLabel.SetText(_summon.Stats.Attack?.ToString() ?? "-");
		_rangeLabel.SetText(_summon.Stats.Range?.ToString() ?? "-");
	}
}