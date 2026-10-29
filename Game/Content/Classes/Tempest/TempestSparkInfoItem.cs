using System.Linq;
using Godot;

public partial class TempestSparkInfoItem : InfoItem<TempestSparkInfoItem.Parameters>
{
	public class Parameters(TempestSpark hexObject) : InfoItemParameters<TempestSpark>(hexObject)
	{
		public override string ScenePath => "res://Content/Classes/Tempest/TempestSparkInfoItem.tscn";
	}

	[Export]
	private Label _sparkCountLabel;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_sparkCountLabel.SetText($"x {parameters.HexObject.Hex.GetHexObjectsOfType<TempestSpark>().Count()}");
	}
}