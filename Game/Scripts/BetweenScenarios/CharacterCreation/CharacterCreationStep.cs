using Godot;
using GTweensGodot.Extensions;

public partial class CharacterCreationStep : Control
{
	protected CharacterCreationOverlay _characterCreationOverlay;

	public bool Active { get; private set; }

	public virtual bool ConfirmButtonActive => false;

	public override void _Ready()
	{
		base._Ready();

		SetModulate(Colors.Transparent);
		Hide();
	}

	public void Init(CharacterCreationOverlay characterCreationOverlay)
	{
		_characterCreationOverlay = characterCreationOverlay;
	}

	public virtual void Activate()
	{
		Active = true;

		Show();
		this.TweenModulateAlpha(1f, 0.3f).Play();
	}

	public virtual void Deactivate()
	{
		Active = false;

		this.TweenModulateAlpha(0f, 0.3f).OnComplete(Hide).Play();
	}
}