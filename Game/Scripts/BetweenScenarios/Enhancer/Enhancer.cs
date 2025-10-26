using Godot;
using GTweens.Builders;

public partial class Enhancer : BetweenScenariosAction
{
	[Export]
	private SubViewport _subViewport;

	[Export]
	private AnimationPlayer _animationPlayer;
	[Export]
	private StringName _moveInAnimationName;
	[Export]
	private StringName _moveOutAnimationName;

	[Export]
	private Node3D _3dRoot;
	[Export]
	private Node3D _crystalBall;

	protected override bool SelectCharacter => true;

	protected override void AnimateIn(GTweenSequenceBuilder sequenceBuilder)
	{
		base.AnimateIn(sequenceBuilder);

		_3dRoot.SetVisible(true);

		_subViewport.SetUpdateMode(SubViewport.UpdateMode.WhenVisible);
		_crystalBall.SetVisible(false);

		sequenceBuilder
			.AppendTime(0.4f)
			.AppendCallback((() =>
			{
				_animationPlayer.Play(_moveInAnimationName);
				this.DelayedCall(() =>
				{
					_crystalBall.SetVisible(true);
				});
			}))
			.AppendTime(1f);
	}

	protected override void AfterAnimateIn()
	{
		base.AfterAnimateIn();
	}

	protected override void AnimateOut(GTweenSequenceBuilder sequenceBuilder)
	{
		sequenceBuilder.AppendTime(1f);

		_animationPlayer.Play(_moveOutAnimationName);

		base.AnimateOut(sequenceBuilder);
	}

	protected override void AfterAnimateOut()
	{
		base.AfterAnimateOut();

		_3dRoot.SetVisible(false);

		_subViewport.SetUpdateMode(SubViewport.UpdateMode.Disabled);
	}
}