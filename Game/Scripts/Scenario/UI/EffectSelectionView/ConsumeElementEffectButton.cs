using Godot;
using System.Collections.Generic;

public partial class ConsumeElementEffectButton : EffectButton<ConsumeElementEffectButton.Parameters>
{
	public class Parameters : EffectButtonParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/EffectButtons/ConsumeElementsEffectButton.tscn";

		public IReadOnlyList<Element> Elements { get; }

		public Parameters(IEnumerable<Element> elements)
		{
			Elements = new List<Element>(elements);
		}

		// For backward compatibility, allow single-element constructor
		public Parameters(Element element)
		{
			Elements = new List<Element> { element };
		}
	}

	[Export]
	private NodePath _elementsContainerPath = "BetterButton/Panel/MarginContainer/ElementsContainer";

	private HBoxContainer _elementsContainer;

	public override void _Ready()
	{
		base._Ready();
		_elementsContainer = GetNode<HBoxContainer>(_elementsContainerPath);
	}

	protected override void Init(Parameters parameters)
	{
		base.Init(parameters);

		// Clean up existing icons
		foreach (Node child in _elementsContainer.GetChildren())
			child.QueueFree();

		foreach (Element element in parameters.Elements)
		{
			TextureRect textureRect = new TextureRect
			{
				Texture = ResourceLoader.Load<Texture2D>(Icons.GetElement(element)),
				ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional
			};
			_elementsContainer.AddChild(textureRect);
		}
	}
}