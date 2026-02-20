using System;
using System.Collections.Generic;
using Godot;

public partial class EffectSelectionView : Control
{
	[Export]
	private Control _effectParent;

	private readonly List<EffectButtonBase> _effects = new List<EffectButtonBase>();

	private EffectCollection _effectCollection;

	public int EffectCount => _effects.Count;

	private event Action<Effect> EffectSelectedEvent;

	public void Open(EffectCollection effectCollection, Action<Effect> onEffectSelected)
	{
		_effectCollection = effectCollection;
		EffectSelectedEvent = onEffectSelected;

		if(_effectCollection == null)
		{
			return;
		}

		foreach(Effect effect in _effectCollection.ApplicableEffects)
		{
			if(!effect.CanApply || effect.EffectType == EffectType.MandatoryAfterOptionals)
			{
				continue;
			}

			PackedScene buttonScene = ResourceLoader.Load<PackedScene>(effect.Subscription.EffectButtonParameters.ScenePath);
			EffectButtonBase effectSelectionEffect = buttonScene.Instantiate<EffectButtonBase>();
			_effectParent.AddChild(effectSelectionEffect);
			effectSelectionEffect.Init(effect);
			effectSelectionEffect.PressedEvent += OnEffectPressed;
			_effects.Add(effectSelectionEffect);
		}
	}

	public void Close()
	{
		foreach(EffectButtonBase effect in _effects)
		{
			effect.Destroy();
		}

		_effects.Clear();
	}

	private void OnEffectPressed(EffectButtonBase effect)
	{
		EffectSelectedEvent?.Invoke(effect.Effect);
	}
}