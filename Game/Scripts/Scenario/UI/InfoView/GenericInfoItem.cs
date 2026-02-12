using Godot;

public partial class GenericInfoItem : InfoItem<GenericInfoItem.Parameters>
{
	public class Parameters(
		HexObject hexObject, string title, string description,
		float xOffset = 0f, float yOffset = 0f,
		float? sceneVerticalSize = null)
		: InfoItemParameters<HexObject>(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/GenericInfoItem.tscn";

		public string Title { get; } = title;
		public string Description { get; } = description;
		public float XOffset { get; } = xOffset;
		public float YOffset { get; } = yOffset;
		public float? SceneVerticalSize { get; } = sceneVerticalSize;
	}

	[Export]
	private Control _container;
	[Export]
	private Control _sceneAnchor;
	[Export]
	private RichTextLabel _descriptionLabel;

	[Export]
	private InfoExtraEffectsView _infoExtraEffectsView;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		if(parameters.SceneVerticalSize.HasValue)
		{
			_container.SetCustomMinimumSize(new Vector2(_container.CustomMinimumSize.X, parameters.SceneVerticalSize.Value));
		}

		_titleLabel.SetText(parameters.Title);
		_descriptionLabel.SetText(parameters.Description);

		PackedScene overlayTileScene = ResourceLoader.Load<PackedScene>(parameters.HexObject.SceneFilePath);
		Node2D instance = overlayTileScene.Instantiate<Node2D>();
		_sceneAnchor.AddChild(instance);
		instance.SetPosition(new Vector2(parameters.XOffset, parameters.YOffset));

		ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.SubscribersChangedEvent += OnGenericInfoItemExtraEffectsSubscriptionsChanged;

		OnGenericInfoItemExtraEffectsSubscriptionsChanged();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(GameController.Instance != null)
		{
			ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.SubscribersChangedEvent -= OnGenericInfoItemExtraEffectsSubscriptionsChanged;
		}
	}

	private void OnGenericInfoItemExtraEffectsSubscriptionsChanged()
	{
		ScenarioCheckEvents.GenericInfoItemExtraEffectsCheck.Parameters genericInfoItemExtraEffectsCheckParameters =
			ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.Fire(
				new ScenarioCheckEvents.GenericInfoItemExtraEffectsCheck.Parameters(_parameters.HexObject));

		_infoExtraEffectsView.Update(genericInfoItemExtraEffectsCheckParameters.InfoExtraEffectsParameters);
	}
}