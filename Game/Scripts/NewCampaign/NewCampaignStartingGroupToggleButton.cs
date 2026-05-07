using Godot;

public partial class NewCampaignStartingGroupToggleButton : ToggleButton<NewCampaignStartingGroupToggleButton>
{
	[Export]
	public StartingGroup StartingGroup { get; private set; }

	public new void Init()
	{
		base.Init();
	}

	protected override void ModulateInactiveAlpha(float value)
	{
		this.SetModulateAlpha(Mathf.Lerp(1f, 0.5f, value));
	}
}