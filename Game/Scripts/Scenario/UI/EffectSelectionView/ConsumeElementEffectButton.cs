using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ConsumeElementEffectButton : EffectButton<ConsumeElementEffectButton.Parameters>
{
	public class Parameters : EffectButtonParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/EffectButtons/ConsumeElementsEffectButton.tscn";

		public List<CardElementConsumption> Elements { get; }

		public Parameters(List<CardElementConsumption> elements)
		{
			Elements = elements;
		}

		public Parameters(Element element)
		{
			Elements = [CardElementConsumption.Consume(element)];
		}

		public Parameters(List<Element> elements)
		{
			Elements = [];
			foreach(Element element in elements)
			{
				Elements.Add(CardElementConsumption.Consume(element));
			}
		}
	}

	[Export]
	private Control _singleElementContainer;
	[Export]
	private TextureRect _singleElementTextureRect;

	[Export]
	private Control _multipleElementsContainer;
	[Export]
	private HBoxContainer _elementsContainer;
	[Export]
	private PackedScene _elementIconScene;

	protected override void Init(Parameters parameters)
	{
		//TODO: Have this work with multi-elements other than wild element
		base.Init(parameters);

		if(parameters.Elements.Count == 0)
		{
			Log.Error("Trying to instantiate a consume element effect button without elements to consume.");
			return;
		}
		else if(parameters.Elements.Count == 1)
		{
			_singleElementContainer.SetVisible(true);
			_multipleElementsContainer.SetVisible(false);

			string path = parameters.Elements[0].ConsumableElements.Equals(Elements.All)
				? Icons.WildElement
				: Icons.GetElement(parameters.Elements[0].ConsumableElements.First());

			_singleElementTextureRect.SetTexture(ResourceLoader.Load<Texture2D>(path));
		}
		else
		{
			_singleElementContainer.SetVisible(false);
			_multipleElementsContainer.SetVisible(true);

			foreach(CardElementConsumption element in parameters.Elements)
			{
				TextureRect textureRect = _elementIconScene.Instantiate<TextureRect>();
				_elementsContainer.AddChild(textureRect);

				string path = element.ConsumableElements.Equals(Elements.All)
					? Icons.WildElement
					: Icons.GetElement(element.ConsumableElements.First());

				textureRect.SetTexture(ResourceLoader.Load<Texture2D>(path));
			}

			int separation = 0;
			switch(parameters.Elements.Count)
			{
				case 2:
					separation = -16;
					break;
				case 3:
					separation = -33;
					break;
				case 4:
					separation = -39;
					break;
				case 5:
					separation = -42;
					break;
				case 6:
					separation = -44;
					break;
			}

			_elementsContainer.AddThemeConstantOverride("separation", separation);
		}
	}
}