using Godot;

public partial class SpiritInfoItem : FigureInfoItem<SpiritInfoItem.Parameters>
{
	public class Parameters(Spirit hexObject) : FigureInfoItemParameters(hexObject)
	{
		public override string ScenePath => "res://Content/Classes/SpiritCaller/SpiritInfoItem.tscn";

		public Spirit Spirit { get; } = hexObject;
	}

	[Export]
	private Label _moveLabel;
	[Export]
	private Label _attackLabel;
	[Export]
	private Label _rangeLabel;

	private Spirit _spirit;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_spirit = parameters.Spirit;

		_portraitTexture.SetTexture(_spirit.Texture);
		_portraitBorder.SetSelfModulate(_spirit.OutlineColor);

		_moveLabel.SetText(_spirit.Move?.ToString() ?? "-");
		_attackLabel.SetText(_spirit.Attack?.ToString() ?? "-");
		_rangeLabel.SetText(_spirit.Range?.ToString() ?? "-");
	}
}