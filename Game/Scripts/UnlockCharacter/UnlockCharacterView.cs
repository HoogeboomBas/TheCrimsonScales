using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class UnlockCharacterView : Control
{
	private static readonly Vector2 ReferenceResolution = new Vector2(1920, 1080);
	private static readonly Vector2 FinalMatScale = 0.7f * Vector2.One;

	[Export]
	private SubViewport _subViewport;

	[Export]
	private Node3D _root3D;
	[Export]
	private Camera3D _camera3D;
	[Export]
	private Tuckbox _tuckbox;
	[Export]
	private BetterButton _skipButton;
	[Export]
	private Sprite3D[] _classIconSprites;
	[Export]
	private Sprite3D _classMat3DSprite;

	[Export]
	private Control _classMatContainer;
	[Export]
	private TextureRect _classMatTextureRect;

	[Export]
	private ChoiceButton _continueButton;

	private float _worldUnitsPerPixel;
	private Vector3 _initialMat3DSpritePosition;

	private bool _skipButtonPressed;

	public event Action SkipButtonPressedEvent;
	public event Action ClosedEvent;

	public override void _Ready()
	{
		base._Ready();

		float fovRad = Mathf.DegToRad(_camera3D.Fov);
		float visibleHeight = 2.0f * _camera3D.Position.Z * Mathf.Tan(fovRad * 0.5f);

		_worldUnitsPerPixel = visibleHeight / ReferenceResolution.Y;

		_initialMat3DSpritePosition = _classMat3DSprite.Position;

		Reset();
		//SetVisible(false);

		_skipButton.Pressed += OnSkipButtonPressed;
		_continueButton.BetterButton.Pressed += OnContinuePressed;

		//this.DelayedCall(() => Open(ModelDB.Class<MirefootModel>()), 2f);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		UpdateScale();
	}

	public void Open(ClassModel classModel, CancellationToken cancellationToken)
	{
		OpenAsync(classModel, cancellationToken).Forget();
	}

	public void Skip()
	{
		Reset();

		SetVisible(true);
		SetProcess(true);
		SetModulate(Colors.White);
		_tuckbox.SetVisible(false);
		_classMat3DSprite.SetVisible(false);
		_classMatContainer.SetVisible(true);
		_classMatContainer.SetScale(FinalMatScale);
		_continueButton.SetActive(true);
	}

	private async GDTaskVoid OpenAsync(ClassModel classModel, CancellationToken cancellationToken)
	{
		foreach(Sprite3D classIconSprite in _classIconSprites)
		{
			classIconSprite.SetTexture(classModel.IconTexture);
		}

		_classMatTextureRect.SetTexture(classModel.MatFrontTexture);
		_classMat3DSprite.SetTexture(classModel.MatFrontTexture);

		Reset();

		await GDTask.Delay(0.1f, cancellationToken: cancellationToken);

		SetVisible(true);
		SetProcess(true);
		this.TweenModulateAlpha(1f, 0.5f).Play();
		await GDTask.Delay(0.3f, cancellationToken: cancellationToken);

		_skipButton.SetEnabled(true, false);

		await _tuckbox.AnimateIn(cancellationToken);

		await GDTask.Delay(1f, cancellationToken: cancellationToken);

		_tuckbox.OpenAnimation(cancellationToken).Forget();

		_classMat3DSprite.SetVisible(true);

		GTweenSequenceBuilder.New()
			.AppendTime(0.1f)
			.Append(_classMat3DSprite.TweenPosition(new Vector3(0f, 0.8f, 0f), 0.5f).SetEasing(Easing.OutBack))
			.AppendTime(0.2f)
			.Append(_classMat3DSprite.TweenPosition(_initialMat3DSpritePosition, 0.3f)) //.SetEasing(Easing.OutBack))
			.Build().Play();

		await GDTask.Delay(1.5f, cancellationToken: cancellationToken);

		_classMat3DSprite.SetVisible(false);
		_classMatContainer.SetVisible(true);

		await _classMatContainer.TweenScale(FinalMatScale, 0.5f).SetEasing(Easing.OutBack).PlayAsync(cancellationToken);

		_continueButton.SetActive(true);
	}

	private void Reset()
	{
		_continueButton.SetActive(false);
		SetVisible(false);
		SetProcess(false);
		SetModulate(Colors.Transparent);
		_tuckbox.SetVisible(false);
		_classMatContainer.SetScale(0.4f * Vector2.One);
		_classMatContainer.SetVisible(false);
		_classMat3DSprite.SetPosition(_initialMat3DSpritePosition);
		_classMat3DSprite.SetVisible(false);
		_skipButton.SetEnabled(false, false);
		_skipButtonPressed = false;
	}

	private void UpdateScale()
	{
		Vector2 current = _subViewport.Size;

		float scaleY = current.Y / ReferenceResolution.Y;

		_root3D.SetScale(Vector3.One / scaleY);
	}

	private void OnSkipButtonPressed()
	{
		_skipButtonPressed = true;

		SkipButtonPressedEvent?.Invoke();
	}

	private void OnContinuePressed()
	{
		_continueButton.SetActive(false);
		this.TweenModulateAlpha(0f, 0.5f).OnComplete(() =>
		{
			Reset();

			ClosedEvent?.Invoke();
		}).Play();
	}
}