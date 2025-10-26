using Godot;
using GTweensGodot.Extensions;

public partial class NewCampaignStep : Control
{
	public virtual bool ConfirmButtonActive => false;

	public bool Active { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		SetModulate(Colors.Transparent);
		Hide();
	}

	public virtual void Activate()
	{
		Active = true;

		Show();
		this.TweenModulateAlpha(1f, 0.5f).Play();
	}

	public virtual void Deactivate()
	{
		Active = false;

		this.TweenModulateAlpha(0f, 0.5f).OnComplete(Hide).Play();
	}
}