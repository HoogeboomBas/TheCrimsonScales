﻿using System;
using Godot;
using GTweens.Builders;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ElementsViewElement : Control
{
	[Export]
	private TextureProgressBar _textureProgressBar;

	private ElementState _state;
	private bool _infusing;

	private GTween _stateTransitionTween;
	private GTween _infusingTween;
	private GTween _visibilityTween;

	public override void _Ready()
	{
		base._Ready();

		_textureProgressBar.TweenSelfModulateAlpha(0f, 0f).Play(true);
		_textureProgressBar.Value = 0f;
		Hide();
	}

	public void SetState(ElementState state)
	{
		const float animationDuration = 0.2f;

		_stateTransitionTween?.Kill();

		switch(state)
		{
			case ElementState.Inert:
				_stateTransitionTween = _textureProgressBar.TweenValue(0f, animationDuration).PlayFastForwardable();
				break;
			case ElementState.Waning:
				_stateTransitionTween = _textureProgressBar.TweenValue(0.5f, animationDuration).PlayFastForwardable();
				break;
			case ElementState.Strong:
				_stateTransitionTween = _textureProgressBar.TweenValue(1f, animationDuration).PlayFastForwardable();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}

		_state = state;

		UpdateVisibility();
	}

	public void SetInfusing(bool infusing)
	{
		_infusing = infusing;
		_infusingTween?.Kill();

		if(_infusing)
		{
			_infusingTween = GTweenSequenceBuilder.New()
				.Append(_textureProgressBar.TweenScale(1.2f, 1f))
				.Append(_textureProgressBar.TweenScale(1f, 1f))
				.Build().SetMaxLoops().Play();
		}
		else
		{
			_infusingTween = _textureProgressBar.TweenScale(1f, 0.2f).PlayFastForwardable();
		}

		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		const float animationDuration = 0.2f;

		_visibilityTween?.Kill();
		if(!_infusing && _state == ElementState.Inert)
		{
			_visibilityTween = _textureProgressBar.TweenSelfModulateAlpha(0f, animationDuration).OnComplete(Hide).PlayFastForwardable();
		}
		else
		{
			Show();
			_visibilityTween = _textureProgressBar.TweenSelfModulateAlpha(1f, animationDuration).PlayFastForwardable();
		}
	}
}