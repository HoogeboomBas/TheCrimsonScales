using System.Collections.Generic;
using Godot;

public partial class ConsumeElementEffectButton : EffectButton<ConsumeElementEffectButton.Parameters>
{
	public class Parameters : EffectButtonParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/EffectButtons/ConsumeElementsEffectButton.tscn";

		public IReadOnlyList<Element> Elements { get; }

		public Parameters(IReadOnlyList<Element> elements)
		{
			Elements = elements;
		}

		public Parameters(Element element)
		{
			Elements = [element];
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
		base.Init(parameters);

		if(parameters.Elements.Count == 0)
		{
			Log.Error("Trying to instantiate a consume element effect button without elements to consume.");
			return;
		}

		if(parameters.Elements.Count == 1)
		{
			_singleElementContainer.SetVisible(true);
			_multipleElementsContainer.SetVisible(false);

			_singleElementTextureRect.SetTexture(ResourceLoader.Load<Texture2D>(Icons.GetElement(parameters.Elements[0])));
		}
		else
		{
			_singleElementContainer.SetVisible(false);
			_multipleElementsContainer.SetVisible(true);

			foreach(Element element in parameters.Elements)
			{
				TextureRect textureRect = _elementIconScene.Instantiate<TextureRect>();
				_elementsContainer.AddChild(textureRect);
				textureRect.SetTexture(ResourceLoader.Load<Texture2D>(Icons.GetElement(element)));
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