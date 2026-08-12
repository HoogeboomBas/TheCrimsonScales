using Godot;

public partial class MarkerInfoItem : InfoItem<MarkerInfoItem.Parameters>
{
	public class Parameters(Marker marker) : InfoItemParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/MarkerInfoItem.tscn";

		public Marker Marker { get; } = marker;
	}

	[Export]
	private Marker _marker;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_marker.MarkerType = parameters.Marker.MarkerType;
	}
}