using System.Collections.Generic;
using System.Linq;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class EnvelopeB : Control
{
	private const int DonationCountPerSet = 45;
	private static Vector3 AnimationAwayPosition = new Vector3(3f, 0f, -5f);
	private static Vector3 AnimationAwayRotation = new Vector3(0f, -30f, 0f);

	[Export]
	private PackedScene _normalCircleScene;
	[Export]
	private PackedScene _yellowCircleScene;
	[Export]
	private Control _circleParent;

	[Export]
	public Node3D EnvelopeB3DRoot;
	[Export]
	public SubViewport SubViewport;
	[Export]
	private Node3D _animationContainer;

	private readonly List<EnvelopeBCircle> _circles = new List<EnvelopeBCircle>();

	public override void _Ready()
	{
		base._Ready();

		//TODO: Support multiple (2) sets of 45 circles/checkmarks

		int donationsCount = BetweenScenariosController.Instance.SavedCampaign.SanctuaryOfTheGreatOak.TotalDonationCount;
		for(int i = 0; i < DonationCountPerSet; i++)
		{
			int number = i + 1;
			PackedScene circleScene = SavedSanctuaryOfTheGreatOak.DonationYellowNumbers.Contains(number) ? _yellowCircleScene : _normalCircleScene;
			EnvelopeBCircle circle = circleScene.Instantiate<EnvelopeBCircle>();
			_circleParent.AddChild(circle);
			circle.Init(number * 10, i < donationsCount);
			_circles.Add(circle);
		}

		_animationContainer.SetPosition(AnimationAwayPosition);
		_animationContainer.SetRotationDegrees(AnimationAwayRotation);
	}

	public void Donate()
	{
		UpdateChecks();
	}

	public void AnimateIn()
	{
		_animationContainer.SetPosition(AnimationAwayPosition);
		_animationContainer.SetRotationDegrees(AnimationAwayRotation);
		_animationContainer.TweenPosition(Vector3.Zero, 0.8f).SetEasing(Easing.OutCubic).Play();
		_animationContainer.TweenRotation(Vector3.Zero, 0.8f).SetEasing(Easing.OutCubic).Play();
	}

	public void AnimateOut()
	{
		_animationContainer.TweenPosition(AnimationAwayPosition, 0.8f).SetEasing(Easing.InOutSine).Play();
		_animationContainer.TweenRotationDegrees(AnimationAwayRotation, 0.8f).SetEasing(Easing.OutCubic).Play();
	}

	private void UpdateChecks()
	{
		int donationsCount = BetweenScenariosController.Instance.SavedCampaign.SanctuaryOfTheGreatOak.TotalDonationCount;
		for(int i = 0; i < donationsCount; i++)
		{
			_circles[i].Check();
		}
	}
}