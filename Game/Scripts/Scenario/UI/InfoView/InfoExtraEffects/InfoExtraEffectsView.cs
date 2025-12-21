using System.Collections.Generic;
using Godot;

public partial class InfoExtraEffectsView : Control
{
	[Export]
	private Control _effectParent;

	private readonly List<InfoExtraEffectBase> _extraEffects = new List<InfoExtraEffectBase>();

	public void Update(List<InfoExtraEffectParameters> parametersList)
	{
		foreach(InfoExtraEffectBase extraEffect in _extraEffects)
		{
			extraEffect.QueueFree();
		}

		_extraEffects.Clear();

		SetVisible(parametersList.Count > 0);

		foreach(InfoExtraEffectParameters parameters in parametersList)
		{
			PackedScene scene = ResourceLoader.Load<PackedScene>(parameters.ScenePath);
			InfoExtraEffectBase extraEffect = scene.Instantiate<InfoExtraEffectBase>();
			_effectParent.AddChild(extraEffect);
			extraEffect.Init(parameters);
			_extraEffects.Add(extraEffect);
		}
	}
}