using System;
using Godot;
using GTweens.Builders;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ElementsViewElement : Control
{
	[Export]
	private Control _container;

	[Export]
	private TextureProgressBar _textureProgressBar;

	[Export]
	private Control _lineContainer;
	[Export]
	private Control _line;

	private ElementState _state;
	private bool _infusing;

	private GTween _stateTransitionTween;
	private GTween _infusingTween;
	private GTween _visibilityTween;

	public override void _Ready()
	{
		base._Ready();

		_container.SetPivotOffset(_container.Size * 0.5f);
		_container.TweenModulateAlpha(0f, 0f).Play(true);
		SetNormalizedFill(0f);

		Hide();
	}

	public void SetState(ElementState state)
	{
		const float animationDuration = 0.2f;

		_stateTransitionTween?.Kill();

		switch(state)
		{
			case ElementState.Inert:
				_stateTransitionTween = CustomGTweenExtensions.Tween((float)_textureProgressBar.Value, 0f, SetNormalizedFill, animationDuration)
					.PlayFastForwardable();
				break;
			case ElementState.Waning:
				_stateTransitionTween = CustomGTweenExtensions.Tween((float)_textureProgressBar.Value, 0.5f, SetNormalizedFill, animationDuration)
					.PlayFastForwardable();
				break;
			case ElementState.Strong:
				_stateTransitionTween = CustomGTweenExtensions.Tween((float)_textureProgressBar.Value, 1f, SetNormalizedFill, animationDuration)
					.PlayFastForwardable();
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
				.Append(_container.TweenScale(1.2f, 1f))
				.Append(_container.TweenScale(1f, 1f))
				.Build().SetMaxLoops().Play();
		}
		else
		{
			_infusingTween = _container.TweenScale(1f, 0.2f).PlayFastForwardable();
		}

		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		const float animationDuration = 0.2f;

		_visibilityTween?.Kill();
		if(!_infusing && _state == ElementState.Inert)
		{
			_visibilityTween = _container.TweenModulateAlpha(0f, animationDuration).OnComplete(Hide).PlayFastForwardable();
		}
		else
		{
			Show();
			_visibilityTween = _container.TweenModulateAlpha(1f, animationDuration).PlayFastForwardable();
		}
	}

	private void SetNormalizedFill(float fill)
	{
		_textureProgressBar.SetValue(fill);
		_line.SetPosition(new Vector2(_line.Position.X, (1f - fill) * _lineContainer.Size.Y));
	}
}