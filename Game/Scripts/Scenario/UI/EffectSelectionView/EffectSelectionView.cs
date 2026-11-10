using System;
using System.Collections.Generic;
using Godot;

public partial class EffectSelectionView : Control
{
	[Export]
	private Control _effectParent;

	private readonly List<EffectButtonBase> _effects = new List<EffectButtonBase>();

	private EffectCollection _effectCollection;
	private bool _effectSelected;

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

			EffectButtonBase effectSelectionEffect =
				SceneLoader.InstantiateScene<EffectButtonBase>(effect.Subscription.EffectButtonParameters.ScenePath);
			_effectParent.AddChild(effectSelectionEffect);
			effectSelectionEffect.Init(effect);
			effectSelectionEffect.PressedEvent += OnEffectPressed;
			_effects.Add(effectSelectionEffect);
		}

		_effectSelected = false;
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
		if(_effectSelected || !_effects.Contains(effect))
		{
			return;
		}

		_effectSelected = true;

		EffectSelectedEvent?.Invoke(effect.Effect);
	}
}