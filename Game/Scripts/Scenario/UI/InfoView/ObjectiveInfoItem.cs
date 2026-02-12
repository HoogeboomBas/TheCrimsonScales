using Godot;

public partial class ObjectiveInfoItem : FigureInfoItem<ObjectiveInfoItem.Parameters>
{
	public class Parameters(Objective hexObject) : FigureInfoItemParameters(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/ObjectiveInfoItem.tscn";

		public Objective Objective { get; } = hexObject;
	}

	[Export]
	private Control _sceneAnchor;

	private Objective _objective;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_objective = parameters.Objective;

		PackedScene overlayTileScene = ResourceLoader.Load<PackedScene>(parameters.HexObject.SceneFilePath);
		Objective instance = overlayTileScene.Instantiate<Objective>();
		_sceneAnchor.AddChild(instance);
		instance.SetScale(0.6f * (_objective.Hexes.Length > 1 ? 0.5f : 1f) * Vector2.One);
		float xOffset = _objective.Hexes.Length > 1 ? -Map.HexWidth / (2 / instance.Scale.X) : 0;
		float yOffset = _objective.Hexes.Length == 3 ? 25f : 0;
		instance.SetPosition(new Vector2(xOffset, yOffset));
		FigureViewComponent figureViewComponent = instance.GetChildOfType<FigureViewComponent>();
		figureViewComponent.SetVisible(false);
	}
}