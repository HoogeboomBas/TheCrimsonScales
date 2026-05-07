using Godot;
using GTweens.Builders;
using GTweensGodot.Extensions;

public abstract partial class BetweenScenariosAction : Control
{
	[Export]
	public BetweenScenariosActionButton Button { get; private set; }

	protected bool _transitioning;

	public bool Active { get; private set; }

	public bool Transitioning => _transitioning || CustomTransitioning;

	protected abstract bool SelectCharacter { get; }
	protected virtual bool CustomTransitioning => false;

	public override void _Ready()
	{
		base._Ready();

		SetVisible(false);
	}

	// public void SetActive(bool active, BetweenScenariosAction previousActiveAction)
	// {
	// }

	public void Activate(BetweenScenariosAction previousActiveAction)
	{
		Active = true;

		Button.SetSelected(true);

		_transitioning = true;
		GTweenSequenceBuilder sequenceBuilder = GTweenSequenceBuilder.New();

		AnimateIn(sequenceBuilder, previousActiveAction);

		sequenceBuilder.AppendCallback(() => _transitioning = false);
		sequenceBuilder.AppendCallback(AfterAnimateIn);
		sequenceBuilder.Build().Play();

		BetweenScenariosController.Instance.CharacterPortraitManager.SetSelectionMode(SelectCharacter);
	}

	public void Deactivate()
	{
		Active = false;

		Button.SetSelected(false);

		_transitioning = true;
		GTweenSequenceBuilder sequenceBuilder = GTweenSequenceBuilder.New();

		AnimateOut(sequenceBuilder);

		sequenceBuilder.AppendCallback(() => _transitioning = false);
		sequenceBuilder.AppendCallback(AfterAnimateOut);
		sequenceBuilder.Build().Play();
	}

	protected virtual void AnimateIn(GTweenSequenceBuilder sequenceBuilder, BetweenScenariosAction previousActiveAction)
	{
		SetVisible(true);
	}

	protected virtual void AfterAnimateIn()
	{
	}

	protected virtual void AnimateOut(GTweenSequenceBuilder sequenceBuilder)
	{
	}

	protected virtual void AfterAnimateOut()
	{
		SetVisible(false);
	}
}